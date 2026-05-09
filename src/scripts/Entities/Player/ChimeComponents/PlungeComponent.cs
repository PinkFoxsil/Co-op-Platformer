using Godot;
using System;

public partial class PlungeComponent : Component
{
	[Export] public float slamForce = 800f;
	[Export] public int attackDamage = 1;

	private bool _isSlamming;
	private Hitbox _plungeHitbox;
	
	private Player _character;
	private InputComponent _input;
	
	public override void Init(Entity entity)
	{
		base.Init(entity);

		_character = (Player) entity.node;
		_input = (InputComponent) entity.GetComponent(typeof(InputComponent));

		Node2D hitboxes = entity.node.GetNode<Node2D>("Hitboxes");
		_plungeHitbox = hitboxes.GetNode<Hitbox>("AirDownHitbox");
		
	}

	public override void PhysicsProcess(float dt)
	{
		if (_isSlamming && _character.IsOnFloor()) {
			_isSlamming = false;
			_plungeHitbox.Deactivate();
		}
		
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
		return !_isSlamming;
	}

	private void Attack()
	{
		CardinalDirection dir = DirectionUtility.GetCardinalDirection(_input.mouseRelativePosition);

		if (dir == CardinalDirection.DOWN && !_character.IsOnFloor())
		{
			_isSlamming = true;
			_character.Velocity = new Vector2(_character.Velocity.X, slamForce);
			_plungeHitbox.Activate();
		}
	}
}
