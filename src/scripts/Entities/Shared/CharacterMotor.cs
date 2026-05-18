using Godot;
using System.Collections.Generic;
using System.Linq;

public struct VelocityRequest
{
	public object source;
	public Vector2 velocity;
	public int priority;
	public bool persistent;
	public Timer timer;
}

public struct GravityRequest
{
	public object source;
	public float multiplier;
	public int priority;
	public bool persistent;
	public Timer timer;
}

public partial class CharacterMotor : Node
{

	[ExportCategory("Gravity")]
	[Export] public float gravity = 1632f;

	[ExportCategory("Clamps")]
	[Export] public float maxFallSpeed = 1000f;

	private float highestPoint = 500000000;
	
	// Velocity contributors
	private readonly List<VelocityRequest> _velocityRequests = new();
	private readonly List<GravityRequest> _gravityRequests = new();

	private Player _player;

	public void Init(Player player)
	{
		_player = player;
	}

	// Velocity Requests
	public VelocityRequest RequestVelocity(object source, Vector2 velocity, int priority = 0, bool persistent = false, Timer timer = null)
	{
		VelocityRequest velocityRequest = new() { source = source, velocity = velocity, priority = priority, persistent = persistent, timer = timer};
		_velocityRequests.Add(velocityRequest);
		return velocityRequest;
	}

	public void RemoveVelocityRequest(VelocityRequest velocityRequest)
	{
		_velocityRequests.Remove(velocityRequest);
	}

	public void ClearVelocitySource(object source)
	{
		_velocityRequests.RemoveAll(r => r.source == source);
	}

	public void ClearVelocityRequests()
	{
		_velocityRequests.Clear();
	}

	// Gravity Multipliers
	public GravityRequest RequestGravityMultiplier(object source, float multiplier, int priority = 0, bool persistent = false, Timer timer = null)
	{
		GravityRequest gravityRequest = new GravityRequest{
			source = source,
			multiplier = multiplier,
			priority = priority,
			persistent = persistent,
			timer = timer
		}; 

		_gravityRequests.Add(gravityRequest);
		return gravityRequest;
	}

	public void RemoveGravityMultiplier(GravityRequest gravityRequest)
	{
		_gravityRequests.Remove(gravityRequest);
	}

	public void ClearGravitySource(object source)
	{
		_gravityRequests.RemoveAll(r => r.source == source);
	}

	public void ClearGravityRequests()
	{
		_gravityRequests.Clear();
	}

	public void Tick(float dt)
	{
		TickTimers(dt);
		
		Vector2 velocity = _player.Velocity;
		GD.Print(velocity.Y);
		float gravityMultiplier = 1f;
		if (_gravityRequests.Count > 0)
		{
			int maxPriority = _gravityRequests.Max(r => r.priority);

			foreach (GravityRequest request in _gravityRequests)
			{
				if (request.priority == maxPriority)
				{
					gravityMultiplier *= request.multiplier;
				}
			}
		}

		velocity.Y += gravity * gravityMultiplier * dt;
		velocity.Y = Mathf.Min(velocity.Y, maxFallSpeed);

		if (_velocityRequests.Count > 0)
		{
			int maxPriority = _velocityRequests.Max(r => r.priority);

			foreach (VelocityRequest request in _velocityRequests)
			{
				if (request.priority == maxPriority)
				{
					velocity += request.velocity;
				}
			}
		}

		_player.Velocity = velocity;
		CleanupExpiredRequests();
		CleanupNonPersistentRequests();
	}

	private void TickTimers(float dt)
	{
		foreach (VelocityRequest request in _velocityRequests)
		{
			request.timer?.Tick(dt);
		}

		foreach (GravityRequest request in _gravityRequests)
		{
			request.timer?.Tick(dt);
		}
	}

	private void CleanupExpiredRequests()
	{
		_velocityRequests.RemoveAll(r =>
			r.timer != null && r.timer.HasStopped
		);

		_gravityRequests.RemoveAll(r =>
			r.timer != null && r.timer.HasStopped
		);
	}

	private void CleanupNonPersistentRequests()
	{
		_velocityRequests.RemoveAll(r =>
			!r.persistent
		);

		_gravityRequests.RemoveAll(r =>
			!r.persistent
		);
	}
}
