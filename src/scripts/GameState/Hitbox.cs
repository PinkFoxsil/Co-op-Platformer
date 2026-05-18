using Godot;

public partial class Hitbox : Area2D
{
	[Export] public bool startsActive = false;
	[Export] public bool staysActive = false;

	public ActionOrchestrator Orchestrator { get; private set; }

	private CollisionShape2D _collisionShape;
	private float _lifetimeTimer;
	private bool active;
	
	public override void _Ready()
	{	
		Orchestrator = GetNode<ActionOrchestrator>("ActionOrchestrator");
		Orchestrator.Init(this);

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
		
		Orchestrator.PrePhysicsUpdate(dt);
		Orchestrator.PhysicsUpdate(dt);
		Orchestrator.PostPhysicsUpdate(dt);

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
