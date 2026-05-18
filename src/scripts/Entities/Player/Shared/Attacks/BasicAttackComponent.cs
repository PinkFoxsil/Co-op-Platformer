using Godot;
using System;

public partial class BasicAttackComponent : AttackComponent
{
	private CardinalDirection _attackDirection;
	private Hitbox _attackHitbox;

	public override void Init(Node owner)
	{
		base.Init((Player) owner);
		_attackHitbox = this.GetNode<Hitbox>("Hitbox");
	}

	public override bool AttackTriggered()
	{
		if (!_input.current.attack1Held)
		{
			return false;
		}
		
		CardinalDirection direction = DirectionUtility.GetCardinalDirection(_input.current.mouseRelativePosition);
		if (direction == CardinalDirection.DOWN)
		{
			return false;
		}

		_attackDirection = direction;
		return true;
	}

	protected override void ExecuteAttack()
	{
		RotateHitbox(_attackDirection);
		_attackHitbox.Activate(attackDuration);
	}

	private void RotateHitbox(CardinalDirection direction)
	{
		Vector2 vector = DirectionUtility.ToVector(direction);
		_attackHitbox.Rotation = vector.Angle();
	}
}
