using Godot;
using System;

public partial class Hitbox : Area2D
{
    private Entity<Area2D> _entity;
	
    public Entity<Area2D> entity => _entity;

	private CollisionShape2D _collisionShape;
	private float _lifetimeTimer;
	private bool active;

	public override void _Ready()
	{
		_entity = new Entity<Area2D>(this);
		_entity.RegisterChildren();
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		Deactivate()
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!active)
		{
			return;
		}

		float dt = (float) delta;
		_lifetimeTimer -= dt;
		
		_entity.PrePhysicsProcess(dt);
		_entity.PhysicsProcess(dt);
		_entity.PostPhysicsProcess(dt);

		if (_lifetimeTimer <= 0f)
		{
			Deactivate();
		}
	}

	public void Activate(float duration)
	{
		_lifetimeTimer = duration;
		active = true

		_collisionShape.SetDeferred(
			"disabled",
			false
		);

		Monitoring = true;
		Monitorable = true;
	}

	public void Deactivate()
	{
		_lifetimeTimer = 0f;
		active = false

		_collisionShape.SetDeferred(
			"disabled",
			true
		);

		Monitoring = false;
		Monitorable = false;
	}
}