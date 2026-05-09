using Godot;
using System;

public partial class GroundSlamComponent
{
	[Export] public float attackCooldown = 1f;
	[Export] public float attackDuration = 0.5f;
	[Export] public int attackDamage = 1;

	private float _attackCooldownTimer;

	private Hitbox _groundSlamHitbox;
	
	private Player _character;
	private InputSingleton _input;
	
	public GroundSlamComponent(CharacterBody2D character, Hitbox groundSlamHitbox)
	{
		_character = (Player) character;
		_input = InputSingleton.of();

		Node2D hitboxes = character;
		_groundSlamHitbox = groundSlamHitbox;
	}

	public override void PhysicsProcess(float dt)
	{
		_attackCooldownTimer -= dt;

		if (_input == null)
		{
			return;
		}

		if (_input.attack1Pressed && CanAttack())
		{
			Attack();
		}
	}

	public virtual bool CanAttack()
	{
		return _attackCooldownTimer <= 0f;
	}

	private void Attack()
	{
		_attackCooldownTimer = attackCooldown;
		CardinalDirection dir = DirectionUtility.GetCardinalDirection(_input.mouseRelativePosition);

		if (dir == CardinalDirection.DOWN && _character.IsOnFloor())
		{
			_groundSlamHitbox.Activate(attackDuration);

		}
	}
}
