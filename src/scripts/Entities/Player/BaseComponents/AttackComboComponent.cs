using Godot;
using System;

public partial class AttackComboComponent : Component
{
	[Export] public float attackCooldown = 1f;
	[Export] public float attackDuration = 0.5f;
	[Export] public int attackDamage = 1;

	private float _attackCooldownTimer;

	private Hitbox _attackComboHitbox;

	private InputComponent _input; 
	
	public override void Init(Node parentNode)
	{
		base.Init(parentNode);

		Player _character = (Player) parentNode;
		_input = (InputComponent) _character.ComponentList.GetComponent(typeof(InputComponent));

		Node2D hitboxes = parentNode.GetNode<Node2D>("Hitboxes");
		_attackComboHitbox = hitboxes.GetNode<Hitbox>("AttackComboHitbox");
		GD.Print(_attackComboHitbox);
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

		if (dir != CardinalDirection.DOWN)
		{
			RotateHitbox(dir);
			_attackComboHitbox.Activate(attackDuration);

		}
	}

	private void RotateHitbox(CardinalDirection dir)
	{
		Vector2 toVector = DirectionUtility.ToVector(dir);
		_attackComboHitbox.Rotation = toVector.Angle();
	}
}