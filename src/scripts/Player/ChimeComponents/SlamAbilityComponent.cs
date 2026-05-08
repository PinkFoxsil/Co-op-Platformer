using Godot;

public partial class SlamAbilityComponent : AttackComponent
{
	[Export] public float slamForce = 800f;
	[Export] public int damage = 1;

	private bool _isSlamming;
	private Area2D _airAttackHitbox;

	private CharacterBody2D _character;

	public override void Init(Entity<CharacterBody2D> entity)
	{
		base.Init(entity);
		_character = entity.node;
		_airAttackHitbox = _character.GetNode<Node2D>("Hitboxes").GetNode<Area2D>("Air");
	}

	public override bool CanAttack()
	{
		if (_isSlamming)
		{
			return false;
		}
		
		return base.CanAttack();
	}

	public override CardinalDirection DetermineAttackDirection(InputComponent input)
	{
		Vector2 mouse = input.mouseRelativePosition;

		// Doesn't allow down attacks while on the ground.
		if (_character.IsOnFloor())
		{
			if (mouse.Y > 0 || Mathf.Abs(mouse.X) > -mouse.Y)
			{
				return mouse.X > 0 ?
					CardinalDirection.RIGHT :
					CardinalDirection.LEFT;
			}
			else
			{
				return CardinalDirection.UP;
			}
		}
		else
		{
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
	}

	public override void AttackDown()
	{
		_isSlamming = true;
		_character.Velocity = new Vector2(_character.Velocity.X, slamForce);
	}
}
