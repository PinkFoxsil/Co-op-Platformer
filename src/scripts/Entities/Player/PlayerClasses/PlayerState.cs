using Godot;
using System;

public class PlayerState
{
	public PlayerStateTag stateTag = PlayerStateTag.Neutral;
	private Timer timer = new Timer();

	public PlayerState() {}

	public void Start()
	{ 
		timer.Start();
	}

	public void Start(float duration)
	{
		timer.Start(duration);
	}

	public void Stop()
	{
		timer.Stop();
		stateTag = PlayerStateTag.Neutral;
	}

	public float Tick(float dt)
	{
		float excess = timer.Tick(dt);
		if (timer.hasStopped) {
			stateTag = PlayerStateTag.Neutral;
		}
		return excess;
	}

	public void AddTime(float addedTime)
	{
	   timer.AddTime(addedTime);
	}
}
