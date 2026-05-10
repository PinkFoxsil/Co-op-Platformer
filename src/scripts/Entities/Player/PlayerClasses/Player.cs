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
	private ComponentList _componentList;

	public ComponentList ComponentList => _componentList;

	private CharacterState _currentState { get; set; }

	public override void _Ready()
	{
		_componentList = new ComponentList(this);
		_componentList.RegisterChildren();

		_currentState = CharacterState.Idle;
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float) delta;
		
		// [TBD] Determine ordering of processing
		_componentList.PrePhysicsProcess(dt);
		_componentList.PhysicsProcess(dt);

		MoveAndSlide();

		_componentList.PostPhysicsProcess(dt);
	}
}