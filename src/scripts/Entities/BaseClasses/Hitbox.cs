using Godot;
using System;

public partial class Hitbox : Area2D
{
	[Export] public bool startsActive = false;
	[Export] public bool staysActive = false;

	private ComponentList _componentList;
	
	public ComponentList ComponentList => _componentList;

	private CollisionShape2D _collisionShape;
	private float _lifetimeTimer;
	private bool active;

	public override void _Ready()
	{
		_componentList = new ComponentList(this);
		_componentList.RegisterChildren();
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

		if (startsActive)
		{
			Activate(0f);
		}
		else
		{
			Deactivate();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!active)
		{
			return;
		}

		float dt = (float) delta;
		
		_componentList.PrePhysicsProcess(dt);
		_componentList.PhysicsProcess(dt);
		_componentList.PostPhysicsProcess(dt);

		if (staysActive)
		{
			return;
		}
		
		_lifetimeTimer -= dt;
		if (_lifetimeTimer <= 0f)
		{
			Deactivate();
		}
	}

	public void Activate(float duration)
	{
		if (staysActive)
		{
			Activate();
			return;
		}
		
		_lifetimeTimer = duration;
		
		Activate();
	}

	public void Activate()
	{
		
		_collisionShape.SetDeferred(
			"disabled",
			false
		);

		active = true;
		Monitoring = true;
		Monitorable = true;
	}

	public void Deactivate()
	{
		_lifetimeTimer = 0f;
		
		_collisionShape.SetDeferred(
			"disabled",
			true
		);

		active = false;
		Monitoring = false;
		Monitorable = false;
	}
}
