using Godot;
using System;

public partial class AttackComponent : Component<Player>
{
	[Export] public float attackCooldown = 1f;
	[Export] public float attackDuration = 0.5f;
	[Export] public int attackDamage = 1;

	private float _attackCooldownTimer;

	private Hitbox _leftAttackHitbox;
	private Hitbox _rightAttackHitbox;
	private Hitbox _upAttackHitbox;

	private float _leftHitboxTimer;
	private float _rightHitboxTimer;
	private float _upHitboxTimer;

	public override void Init(Entity<Player> entity)
	{
		base.Init(entity);

		Node2D hitboxes = entity.node.GetNode<Node2D>("Hitboxes");

		_leftAttackHitbox = hitboxes.GetNode<Hitbox>("LeftHitbox");
		_rightAttackHitbox = hitboxes.GetNode<Hitbox>("RightHitbox");
		_upAttackHitbox = hitboxes.GetNode<Hitbox>("UpHitbox");
	}

	public override void PhysicsProcess(float dt)
	{
		_attackCooldownTimer -= dt;

		InputComponent input = entity.GetComponent<InputComponent>();
		if (input == null)
		{
			return;
		}

		if (input.attack1Pressed && CanAttack())
		{
			Attack(input);
		}
	}

	public virtual bool CanAttack()
	{
		return _attackCooldownTimer <= 0f && false;
	}

	private void Attack(InputComponent input)
	{
		_attackCooldownTimer = attackCooldown;
			
		switch (DetermineAttackDirection(input))
		{
			case CardinalDirection.LEFT:
				AttackLeft();
				break;
			case CardinalDirection.RIGHT:
				AttackRight();
				break;
			case CardinalDirection.UP:
				AttackUp();
				break;
			case CardinalDirection.DOWN:
				AttackDown();
				break;
		}
	}

	public virtual CardinalDirection DetermineAttackDirection(InputComponent input)
	{
		Vector2 mouse = input.mouseRelativePosition;

		if (Mathf.Abs(mouse.X) > Mathf.Abs(mouse.Y))
		{
			return mouse.X > 0 ?
				CardinalDirection.RIGHT :
				CardinalDirection.LEFT;
		}
		else
		{
			return mouse.Y > 0 ?
				CardinalDirection.DOWN :
				CardinalDirection.UP;
		}
	}

	private void AttackLeft()
	{
		_leftAttackHitbox.Activate(attackDuration);
	}

	private void AttackRight()
	{
		_rightAttackHitbox.Activate(attackDuration);
	}

	private void AttackUp()
	{
		_upAttackHitbox.Activate(attackDuration);
	}

	public virtual void AttackDown()
	{
		
	}
}