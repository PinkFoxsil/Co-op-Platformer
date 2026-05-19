using Godot;

public partial class GravityComponent : Node, IActionComponent
{
	[Export] public float maxFallSpeed = 1000f;
	[Export] public float fallMultiplier = 2f;

	private Player _player;
	private CharacterMotor _motor;
	private PlayableCharacterData _characterData;

	public void Init(Node owner)
	{
		Player player = (Player) owner;
		_player = player;
		_motor = player.Motor;

		_characterData = owner.GetNode<PlayableCharacterData>("CharacterData");
	}

	public void PhysicsUpdate(float dt)
	{
		if (_player.Velocity.Y > maxFallSpeed)
		{
			float velocityDifference = maxFallSpeed - _player.Velocity.Y;
			_motor.RequestVelocity(this, new Vector2(0, velocityDifference) * dt);
		}
		else
		{
			if (_player.Velocity.Y > 0)
			{
				_motor.RequestVelocity(this, new Vector2(0, _characterData.Gravity * fallMultiplier) * dt);
			}
			else
			{
				_motor.RequestVelocity(this, new Vector2(0, _characterData.Gravity) * dt);
			}
		}
	}
}
