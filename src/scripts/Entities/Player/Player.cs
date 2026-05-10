using Godot;
using System;

enum CharacterState
{
	Idle,
	Running,
	Jumping,
	Falling,
	Dashing,
	Attacking,
	Stunned
}

enum AttackState
{
	LeftAttack,
	RightAttack,
	UpAttack,
	DownAttack
}

public partial class Player : CharacterBody2D
{
	[Export] public int maxHealth = 5;

	[ExportCategory("Movement")]

	[ExportGroup("Running")]
    [Export] public float moveSpeed = 260f;
    [Export] public float acceleration = 2000f;
    [Export] public float deceleration = 3000f;
	
    [ExportGroup("Jumping")]
    [Export] public float jumpForce = 500f;

    [Export] public float coyoteTime = 0.125f;
    [Export] public float jumpBuffer = 0.1f;

    [Export] public float jumpHangTimeThreshold = 5f;
    [Export] public float jumpHangGravityMultiplier = 0.5f;

	[ExportGroup("Control")]
    [Export] public float airControl = 0.6f;
    [Export] public float groundControl = 1f;

    [ExportGroup("Gravity")]
    [Export] public float gravityScale = 1.4f;
    [Export] public float fallMultiplier = 1.4f;
    [Export] public float maxFallSpeed = 1000f;

	private AttackComboComponent _attackComboComponent;
	private BaseMovementComponent _movementComponent;
	private HealthComponent _healthComponent;

	private CharacterState _currentState { get; set; }

	public Vector2 mouseDirection;
	private Vector2 mouseRelativePosition;

	public override void _Ready()
	{
		Hitbox hitbox = GetNode<Hitbox>("Hitboxes/PlayerHitbox");
		Hitbox attackComboHitbox = GetNode<Hitbox>("Hitboxes/AttackComboHitbox");

		_attackComboComponent = new AttackComboComponent(attackComboHitbox);
		_movementComponent = new BaseMovementComponent(this);
		_healthComponent = new HealthComponent(maxHealth, hitbox);

		_currentState = CharacterState.Idle;

		mouseRelativePosition = mouseWorldPosition - Position;
		mouseDirection = mouseRelativePosition.Normalized();
	}

	public void PhysicsProcess(double delta)
	{
		_movementComponent.PhysicsProcess((float) delta);
		_attackComboComponent.PhysicsProcess((float) delta);
		_healthComponent.PhysicsProcess((float) delta);
		MoveAndSlide();
	}
}
