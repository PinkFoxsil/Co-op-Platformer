using Godot;
using System;

public partial class AttackComboComponent : Component<Player>
{
	[Export] public float attackCooldown = 1f;
	[Export] public float attackDuration = 0.5f;
	[Export] public int attackDamage = 1;

	private float _attackCooldownTimer;

	private Hitbox _attackComboHitbox;
	
	public override void Init(Entity<Player> entity)
	{
		base.Init(entity);

		Node2D hitboxes = entity.node.GetNode<Node2D>("Hitboxes");

		_attackComboHitbox = hitboxes.GetNode<Hitbox>("RightHitbox");
		
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
		return _attackCooldownTimer <= 0f;
	}

	private void Attack(InputComponent input)
	{
		_attackCooldownTimer = attackCooldown;
		CardinalDirection dir = DirectionUtility.GetCardinalDirection(input.mouseRelativePosition);

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