using Godot;

public partial class RunComponent : Node, IActionComponent
{
	[Export] public float moveSpeed = 350f;
	[Export] public float acceleration = 2000f;
	[Export] public float deceleration = 3000f;

	[Export] public float airControl = 0.65f;
	[Export] public float groundControl = 1f;

	private Player _player;
	private CharacterMotor _motor;
	private PlayerInput _input;

	public void Init(Node owner)
	{
		Player player = (Player) owner;
		_player = player;
		_motor = player.Motor;
		_input = player.Input;
	}

	public void PhysicsUpdate(float dt)
	{
		if (!_player.Orchestrator.CanMove())
		{
			return;
		}

		float control = _player.IsOnFloor() ? groundControl : airControl;

		float inputX = _input.current.moveX;
		float targetSpeed = inputX * moveSpeed;
		float currentSpeed = MathUtility.SnapToZero(_player.Velocity.X);
		bool slowingDown = Mathf.IsZeroApprox(inputX) || Mathf.Sign(inputX) != Mathf.Sign(currentSpeed);

		float rate = slowingDown ? deceleration : acceleration;

		float newVelocityX = Mathf.MoveToward(currentSpeed, targetSpeed, rate * control * dt);
		float changeVelocityX = newVelocityX - currentSpeed;

		if (changeVelocityX == 0)
		{
			return;
		}
		
		_motor.RequestVelocity(this, new Vector2(changeVelocityX, 0), priority: 0);
	}
}
