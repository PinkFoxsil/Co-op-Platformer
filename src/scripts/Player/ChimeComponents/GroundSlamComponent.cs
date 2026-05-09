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
	
	public override void Init(Entity entity)
	{
		base.Init(entity);
		
		_character = (Player) entity.node;
		_input = (InputComponent) entity.GetComponent(typeof(InputComponent));

		Node2D hitboxes = entity.node.GetNode<Node2D>("Hitboxes");
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
