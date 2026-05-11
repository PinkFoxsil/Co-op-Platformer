using Godot;
using System;

public class Timer
{
    public float ElapsedTime {get; private set; }
	public float TimeLeft { get; private set; }
    public bool IsInfinite {get; private set;}

    public bool HasStarted {get; private set; }
    public bool HasStopped {get; private set; }
    public bool IsRunning { get; private set; }

    public void Start()
    { 
        RestartCommonState();
        IsInfinite = true;
    }

	public void Start(float duration)
	{
        RestartCommonState(duration);
        IsInfinite = false;
	}

    private void RestartCommonState(float duration = 0f)
    {
        ElapsedTime = 0f;
        TimeLeft = duration;
        IsRunning = true;
        IsInfinite = false;
        HasStarted = true;
        HasStopped = false;
    }

	public float Tick(float dt)
	{
		if (!IsRunning)
		{
			return 0f;
		}

        ElapsedTime += dt;

        if (IsInfinite)
        {
            return 0f;
        }

		TimeLeft -= dt;
        float excess = -TimeLeft;

		if (TimeLeft <= 0)
		{
			TimeLeft = 0;
            Stop();
		}

        return excess;
	}

    public void Stop()
	{
        if (!IsRunning)
        {
            return;
        }

        IsRunning = false;
        TimeLeft = 0;
        HasStopped = true;
	}

    public void AddTime(float addedTime)
    {
        if (!IsRunning || IsInfinite)
        {
            return;
        }

        TimeLeft += addedTime;
    }
}