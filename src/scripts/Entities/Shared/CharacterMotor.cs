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
	[Export] public float jumpHeight = 120f;
	[Export] public float jumpTimeToApex = 0.35f;
	[Export] public float maxFallSpeed = 2400f;
	public float gravity;

	private Player _player;

	public RequestHandler<VelocityRequest> _baseVelocityHandler;
	public RequestHandler<VelocityRequest> _effectVelocityHandler;
	public RequestHandler<GravityRequest> _gravityMultiplierHandler;

	public override void _Ready()
	{
		gravity = 2f * jumpHeight / (jumpTimeToApex * jumpTimeToApex);
		GD.Print(gravity);

		_baseVelocityHandler = new RequestHandler<VelocityRequest>(
			r => r.priority,
			requests =>
			{
				Vector2 result = Vector2.Zero;

				foreach (var r in requests)
					result += r.velocity;

				return new VelocityRequest{velocity = result};
			}
		);

		_effectVelocityHandler = new RequestHandler<VelocityRequest>(
			r => r.priority,
			requests =>
			{
				Vector2 result = Vector2.Zero;

				foreach (var r in requests)
					result += r.velocity;

				return new VelocityRequest{velocity = result};
			}
		);

		_gravityMultiplierHandler = new RequestHandler<GravityRequest>(
			r => r.priority,
			requests =>
			{
				float result = 1f;

				foreach (var r in requests)
					result *= r.multiplier;

				return new GravityRequest{multiplier = result};
			}
		);
	}

	public void Init(Player player)
	{
		_player = player;
	}

	public VelocityRequest RequestBaseVelocity(object source, Vector2 velocity, int priority = 0, bool persistent = false, Timer timer = null)
	{
		return _baseVelocityHandler.Request(new VelocityRequest
		{
			source = source, 
			velocity = velocity, 
			priority = priority,
			persistent = persistent,
			timer = timer
		});
	}

	public VelocityRequest RequestEffectVelocity(object source, Vector2 velocity, int priority = 0, bool persistent = false, Timer timer = null)
	{
		return _effectVelocityHandler.Request(new VelocityRequest
		{
			source = source, 
			velocity = velocity, 
			priority = priority, 
			persistent = persistent, 
			timer = timer
		});
	}

	public GravityRequest RequestGravityMultiplier(object source, float multiplier, int priority = 0, bool persistent = false, Timer timer = null)
	{
		return _gravityMultiplierHandler.Request(new GravityRequest
		{
			source = source,
			multiplier = multiplier,
			priority = priority,
			persistent = persistent,
			timer = timer
		});
	}

	public void Tick(float dt)
	{
		_baseVelocityHandler.Tick(dt, r => r.timer);
		_effectVelocityHandler.Tick(dt, r => r.timer);
		_gravityMultiplierHandler.Tick(dt, r => r.timer);

		Vector2 velocity = _player.Velocity;

		GravityRequest gravityMultiplier = _gravityMultiplierHandler.Resolve(new GravityRequest{multiplier = 1f});
		float gravityInfluence = gravity * gravityMultiplier.multiplier * dt;
		if (velocity.Y < maxFallSpeed)
		{
			velocity.Y = Mathf.Min(velocity.Y + gravityInfluence, maxFallSpeed);
		}

		VelocityRequest baseVelocity = _baseVelocityHandler.Resolve(new VelocityRequest{velocity = Vector2.Zero});
		velocity += baseVelocity.velocity;

		VelocityRequest effectVelocity = _effectVelocityHandler.Resolve(new VelocityRequest{velocity = Vector2.Zero});
		velocity += effectVelocity.velocity;

		_player.Velocity = velocity;

		_baseVelocityHandler.Cleanup(r => r.persistent, r => r.timer);
		_effectVelocityHandler.Cleanup(r => r.persistent, r => r.timer);
		_gravityMultiplierHandler.Cleanup(r => r.persistent, r => r.timer);
	}
}
