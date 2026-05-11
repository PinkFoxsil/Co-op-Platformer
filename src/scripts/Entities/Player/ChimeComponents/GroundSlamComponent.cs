using Godot;
using System;

public partial class GroundSlamComponent : Node, IComponent
{
	[Export] public float attackCooldown = 1f;
	[Export] public float attackDuration = 0.5f;
	[Export] public int attackDamage = 1;

	private float _attackCooldownTimer;

	private Hitbox _groundSlamHitbox;
	
	private Player _character;
	private InputComponent _input;
	private DirectionalAttackComponent _directionalAttack;
	
	public void Init(Node parentNode)
	{

		_character = (Player) parentNode;
		_input = (InputComponent) _character.ComponentList.GetComponent(typeof(InputComponent));
		//_directionalAttack = (DirectionalAttackComponent) _character.ComponentList.GetComponent(typeof(DirectionalAttackComponent));

		Node2D hitboxes = _character.GetNode<Node2D>("Hitboxes");
		_groundSlamHitbox = hitboxes.GetNode<Hitbox>("GroundHitbox");

		//_directionalAttack.attacks[CardinalDirection.DOWN] = Attack;
	}

	public void PhysicsProcess(float dt)
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
