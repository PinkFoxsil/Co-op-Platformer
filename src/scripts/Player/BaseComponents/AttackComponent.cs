using Godot;
using System;

public partial class AttackComponent : Component
{
	[Export] public float attackCooldown = 1f;
	[Export] public float attackDuration = 0.5f;
	[Export] public int attackDamage = 1;

	[Export] bool collisionVisible = false;

	private float _attackCooldownTimer;

	private Area2D _leftAttackHitbox;
	private Area2D _rightAttackHitbox;
	private Area2D _upAttackHitbox;

	private float _leftHitboxTimer;
	private float _rightHitboxTimer;
	private float _upHitboxTimer;

	public override void Init(Entity entity)
	{
		base.Init(entity);

		Node2D hitboxes = entity.GetNode<Node2D>("Hitboxes");
		_leftAttackHitbox = hitboxes.GetNode<Area2D>("Left");
		_rightAttackHitbox = hitboxes.GetNode<Area2D>("Right");
		_upAttackHitbox = hitboxes.GetNode<Area2D>("Up");
	}

	public override void PhysicsProcess(float dt)
	{
		_attackCooldownTimer -= dt;

		if (_leftHitboxTimer <= 0f)
		{
			_leftAttackHitbox.GetNode<CollisionShape2D>("Hitbox").Disabled = true;
		}
		else
		{
			_leftHitboxTimer -= dt;
		}

		if (_rightHitboxTimer <= 0f)
		{
			_rightAttackHitbox.GetNode<CollisionShape2D>("Hitbox").Disabled = true;
		}
		else
		{
			_rightHitboxTimer -= dt;
		}

		if (_upHitboxTimer <= 0f)
		{
			_upAttackHitbox.GetNode<CollisionShape2D>("Hitbox").Disabled = true;
		}
		else
		{
			_upHitboxTimer -= dt;
		}

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
		return _attackCooldownTimer <= 0f;
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
		_leftAttackHitbox.GetNode<CollisionShape2D>("Hitbox").Disabled = false;
		_leftHitboxTimer = attackDuration;
	}

	private void AttackRight()
	{
		_rightAttackHitbox.GetNode<CollisionShape2D>("Hitbox").Disabled = false;
		_rightHitboxTimer = attackDuration;
	}

	private void AttackUp()
	{
		_upAttackHitbox.GetNode<CollisionShape2D>("Hitbox").Disabled = false;
		_upHitboxTimer = attackDuration;
	}

	public virtual void AttackDown()
	{
		
	}
}
