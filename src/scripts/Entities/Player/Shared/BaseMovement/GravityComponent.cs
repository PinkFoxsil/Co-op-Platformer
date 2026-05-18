using Godot;

public partial class GravityComponent : Node, IActionComponent
{
	[Export] public float fallMultiplier = 2f;

	private Player _player;
	private CharacterMotor _motor;

	public void Init(Node owner)
	{
		Player player = (Player) owner;
		_player = player;
		_motor = player.Motor;
	}

	public void PhysicsUpdate(float dt)
	{
		if (_player.Velocity.Y > 0)
		{
			_motor.RequestGravityMultiplier(this, fallMultiplier);
		}
	}
}
