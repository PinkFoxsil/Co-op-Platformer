using Godot;
using System;

public class Timer
{
    public float elapsedTime {get; private set; }
	public float timeLeft { get; private set; }

    public bool isRunning { get; private set; }
    public bool isInfinite {get; private set;}

    public void Start()
    { 
        RestartCommonState();
        isInfinite = true;
    }

	public void Start(float duration)
	{
        RestartCommonState(duration);
        isInfinite = false;
	}

    private void RestartCommonState(float duration = 0f)
    {
        elapsedTime = 0f;
        timeLeft = duration;
        isRunning = true;
        isInfinite = false;
    }

	public void Stop()
	{
        if (!isRunning)
        {
            return;
        }

        isRunning = false;
        timeLeft = 0;
	}

	public float Tick(float dt)
	{
		if (!isRunning)
		{
			return 0f;
		}

        elapsedTime += dt;

        if (isInfinite)
        {
            return 0f;
        }

		timeLeft -= dt;
        float excess = -timeLeft;

		if (timeLeft <= 0)
		{
			timeLeft = 0;
            Stop();
		}

        return excess;
	}

    public void AddTime(float addedTime)
    {
        if (!isRunning || isInfinite)
        {
            return;
        }

        timeLeft += addedTime;
    }
}