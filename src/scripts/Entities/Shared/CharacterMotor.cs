using Godot;
using System.Collections.Generic;
using System.Linq;

public struct VelocityRequest
{
	public Node source;
	public Vector2 velocity;
	public int priority;
	public bool persistent;
	public Timer timer;
}

public partial class CharacterMotor : Node
{
	// Velocity contributors
	private readonly List<VelocityRequest> _velocityRequests = new();

	private Player _player;

	public void Init(Player player)
	{
		_player = player;
	}

	// Velocity Requests
	public VelocityRequest RequestVelocity(Node source, Vector2 velocity, int priority = 0, bool persistent = false, Timer timer = null)
	{
		VelocityRequest velocityRequest = new()
		{
			source = source,
			velocity = velocity,
			priority = priority,
			persistent = persistent,
			timer = timer
		};

		_velocityRequests.Add(velocityRequest);
		return velocityRequest;
	}

	public void RemoveVelocityRequest(VelocityRequest velocityRequest)
	{
		_velocityRequests.Remove(velocityRequest);
	}

	public void ClearVelocitySource(Node source)
	{
		_velocityRequests.RemoveAll(r => r.source == source);
	}

	public void ClearVelocityRequests()
	{
		_velocityRequests.Clear();
	}

	public void Tick(float dt)
	{
		TickTimers(dt);
		
		Vector2 velocity = _player.Velocity;

		if (_velocityRequests.Count > 0)
		{
			int maxPriority = _velocityRequests.Max(r => r.priority);

			foreach (VelocityRequest request in _velocityRequests)
			{
				GD.Print($"Applying velocity {request.velocity} from component {request.source.Name}");
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
	}

	private void CleanupExpiredRequests()
	{
		_velocityRequests.RemoveAll(r =>
			r.timer != null && r.timer.HasStopped
		);
	}

	private void CleanupNonPersistentRequests()
	{
		_velocityRequests.RemoveAll(r =>
			!r.persistent
		);
	}
}
