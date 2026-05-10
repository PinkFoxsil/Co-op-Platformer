using Godot;
using System;

public partial class GroundSlamComponent : Component
{
	[Export] public float attackCooldown = 1f;
	[Export] public float attackDuration = 0.5f;
	[Export] public int attackDamage = 1;

	private float _attackCooldownTimer;

	private Hitbox _groundSlamHitbox;
	
	private Player _character;
	private InputComponent _input;
	
	public override void Init(Node parentNode)
	{
		base.Init(parentNode);

		_character = (Player) parentNode;
		_input = (InputComponent) _character.ComponentList.GetComponent(typeof(InputComponent));

		Node2D hitboxes = _character.GetNode<Node2D>("Hitboxes");
		_groundSlamHitbox = hitboxes.GetNode<Hitbox>("GroundHitbox");
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