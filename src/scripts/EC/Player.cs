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
    private Entity<CharacterBody2D> _entity;

    public Entity<CharacterBody2D> entity => _entity;

	private CharacterState _currentState { get; set; }

	public override void _Ready()
	{
		_entity = new Entity<CharacterBody2D>(this);
		_entity.RegisterChildren();

		_currentState = CharacterState.Idle;
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float) delta;
		
		_entity.PrePhysicsProcess(dt);
		_entity.PhysicsProcess(dt);

		MoveAndSlide();

		_entity.PostPhysicsProcess(dt);
	}
}