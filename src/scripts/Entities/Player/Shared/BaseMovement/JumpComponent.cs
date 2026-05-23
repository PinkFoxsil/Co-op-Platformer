using Godot;
public partial class JumpComponent : Node, IActionComponent
{
	[Export] public float coyoteTime = 0.125f;
	[Export] public float jumpBufferTime = 0.1f;
	[Export] public float jumpCooldownTime = 0.1f;

	private float _jumpForce;

	private Timer _coyoteTimer = new();
	private Timer _jumpBufferTimer = new();
	private Timer _jumpCooldownTimer = new();

	private bool _wasOnFloor;

	private Player _player;
	private CharacterMotor _motor;
	private PlayerInput _input;

	public void Init(Node owner)
	{
		Player player = (Player) owner;
		_player = player;
		_motor = player.Motor;
		_input = player.Input;

		_jumpForce = _motor.gravity * _motor.jumpTimeToApex;
	}

	public void PrePhysicsUpdate(float dt)
	{
		UpdateCoyoteTime(dt);
		UpdateJumpBuffer(dt);
		UpdateJumpCooldownTimer(dt);
	}

	public void PhysicsUpdate(float dt)
	{
		if (!_player.Orchestrator.CanMove())
		{
			return;
		}

		if (_jumpBufferTimer.IsRunning && CanJump())
		{
			Jump();
		}
	}

	private void Jump()
	{
		_coyoteTimer.Stop();
		_jumpBufferTimer.Stop();
		_jumpCooldownTimer.Start(jumpCooldownTime);
		
		_motor.RequestBaseVelocity(this, new Vector2(0, -_jumpForce), priority: 0);
	}

	private bool CanJump()
	{
		return (_player.IsOnFloor() || _coyoteTimer.IsRunning) && !_jumpCooldownTimer.IsRunning;
	}

	private void UpdateCoyoteTime(float dt)
	{
		bool isOnFloor = _player.IsOnFloor();

		if (_wasOnFloor && !isOnFloor && !_jumpCooldownTimer.IsRunning)
		{
			_coyoteTimer.Start(coyoteTime);
		}

		_wasOnFloor = isOnFloor;

		_coyoteTimer.Tick(dt);
	}

	private void UpdateJumpBuffer(float dt)
	{
		if (_input.current.jumpHeld)
		{
			_jumpBufferTimer.Start(jumpBufferTime);
		}
		_jumpBufferTimer.Tick(dt);
	}
	
	private void UpdateJumpCooldownTimer(float dt)
	{
		if (_jumpCooldownTimer.IsRunning) 
		{
			_jumpCooldownTimer.Tick(dt);
		}
	}
}
