using Godot;
using System;

public partial class PlungeComponent : Component<Player>
{
	[Export] public float slamForce = 800f;
	[Export] public int attackDamage = 1;

	private bool _isSlamming;
	private Hitbox _plungeHitbox;
	
	private Player _character;
	
	public override void Init(Entity<Player> entity)
	{
		base.Init(entity);

		_character = entity.node;

		Node2D hitboxes = entity.node.GetNode<Node2D>("Hitboxes");
		_plungeHitbox = hitboxes.GetNode<Hitbox>("AirDownHitbox");
		
	}

	public override void PhysicsProcess(float dt)
	{
		if (_isSlamming && _character.IsOnFloor()) {
			_isSlamming = false;
			_plungeHitbox.Deactivate();
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
		return !_isSlamming;
	}

	private void Attack(InputComponent input)
	{
		CardinalDirection dir = DirectionUtility.GetCardinalDirection(input.mouseRelativePosition);

		if (dir == CardinalDirection.DOWN && !_character.IsOnFloor())
		{
			_isSlamming = true;
			_character.Velocity = new Vector2(_character.Velocity.X, slamForce);
			_plungeHitbox.Activate();
		}
	}
}
