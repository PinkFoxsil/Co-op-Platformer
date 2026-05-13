using Godot;
using System;

public enum MoveState
{
	Idle,
	Running,
	Jumping,
	Falling
}

public partial class BaseMovementComponent : Node, IComponent
{
	[ExportCategory("Movement")]
	[Export] public float moveSpeed = 352f;
	[Export] public float acceleration = 80f;
	[Export] public float deceleration = 160f;

	[ExportCategory("Jumping")]
	[Export] public float jumpHeight = 112f;
	[Export] public float jumpTimeToApex = 0.3f;

	[Export] public float fallMultiplier = 2f;
	[Export] public float maxSpeed = 960f;

	[ExportGroup("Grace Timers")]
	[Export] public float coyoteTime = 0.125f;
	[Export] public float jumpBufferTime = 0.1f;

	[ExportGroup("Jump Hang")]
	[Export] public float jumpHangTimeThreshold = 32f;
	[Export] public float jumpHangGravityMultiplier = 0.5f;
	[Export] public float jumpHangAccelerationMultiplier = 1.1f;

	[ExportCategory("Control")]
	[Export] public float airControl = 0.65f;
	[Export] public float groundControl = 1f;

	public bool movementEnabled;
	public MoveState moveState;

	private float _gravityStrength;
	private float _gravityScale;
	private float _jumpForce;

	private float _runAccelAmount;
	private float _runDeccelAmount;
	private float _accelRate;
	private float _currentGravity;

	private Timer _coyoteTimer = new Timer();
	private Timer _jumpBufferTimer = new Timer();

	private Player _character;
	private InputSingleton _input;

	public void Init(Node parentNode)
	{
		_character = (Player) parentNode;
		_input = InputSingleton.Instance;

		moveState = _character.IsOnFloor() ? MoveState.Idle : MoveState.Falling;

		_gravityStrength = 2*jumpHeight/(jumpTimeToApex*jumpTimeToApex);
		GD.Print(_gravityStrength);
	}

	public void PrePhysicsProcess(float dt)
	{
		UpdateCoyoteTime(dt);
		UpdateJumpBuffer(dt);
	}

	private void UpdateCoyoteTime(float dt)
	{
		if (_character.IsOnFloor())
		{
			_coyoteTimer.Start(coyoteTime);
			return;
		} 

		_coyoteTimer.Tick(dt);
	}

	private void UpdateJumpBuffer(float dt)
	{
		if (_input.jumpPressed)
		{
			_jumpBufferTimer.Start(jumpBufferTime);
			return;
		}

		_jumpBufferTimer.Tick(dt);
	}

	public void PhysicsProcess(float dt)
	{
		if (_character.GetGravity().Y == 0)
		{
			return;
		}

		_gravityScale = _gravityStrength / _character.GetGravity().Y;
		_currentGravity = _character.GetGravity().Y*_gravityScale;
		_jumpForce = _gravityStrength*jumpTimeToApex;

		float inverseDeltaTime = 1/dt;
		_runAccelAmount = inverseDeltaTime*acceleration/moveSpeed;
		_runDeccelAmount = inverseDeltaTime*deceleration/moveSpeed;

		float targetSpeed = _input.inputX * moveSpeed;

		UpdateState();

		if (moveState == MoveState.Idle ||
			moveState == MoveState.Running)
		{
			CheckJump();
			_accelRate = GetAccelRate(targetSpeed)*groundControl;
		}

		if (moveState == MoveState.Jumping ||
			moveState == MoveState.Falling)
		{
			_accelRate = GetAccelRate(targetSpeed)*airControl;
			ApplyJumpHangMovementBoost();
		}
		
		if (moveState == MoveState.Falling)
		{
			ApplyFallMultiplier();
		}

		ApplyHorizontalMovement(dt, targetSpeed);
		ApplyGravity(dt);
		CapSpeed();
		SnapToGround();
	}

	private void UpdateState()
	{
		if (IsGrounded())
		{
			moveState = Mathf.Abs(_character.Velocity.X) < Mathf.Epsilon ?
				MoveState.Idle : MoveState.Running;
		}
		else
		{
			moveState = _character.Velocity.Y >= 0 ? MoveState.Falling : MoveState.Jumping;
		}
	}

	// Similar to IsOnFloor but includes the conditions needed for jumping
	private bool IsGrounded()
	{
		return _character.IsOnFloor() || (_coyoteTimer.IsRunning && _jumpBufferTimer.IsRunning);
	}

	private void CheckJump()
	{
		if (_input.jumpPressed && movementEnabled)
		{
			Jump();
		}
	}

	private void Jump()
	{
		ApplyJumpForce();

		_coyoteTimer.Stop();
		_jumpBufferTimer.Stop();

		moveState = MoveState.Jumping;
	}

	private void ApplyJumpForce()
	{
		_character.Velocity = new Vector2(
			_character.Velocity.X,
			-_jumpForce
		);
	}

	private float GetAccelRate(float targetSpeed)
	{
		return Mathf.Abs(targetSpeed) > 0 ? _runAccelAmount : _runDeccelAmount;
	}

	private void ApplyJumpHangMovementBoost()
	{
		if (Mathf.Abs(_character.Velocity.Y) < jumpHangTimeThreshold)
		{
			_currentGravity *= jumpHangGravityMultiplier;
			_accelRate *= jumpHangAccelerationMultiplier;
		}
	}

	private void ApplyFallMultiplier()
	{
		_currentGravity *= fallMultiplier;
	}

	private void ApplyHorizontalMovement(float dt, float targetSpeed)
	{
		float desiredSpeedDifference = targetSpeed - _character.Velocity.X;
		float movementX = desiredSpeedDifference*_accelRate;

		_character.Velocity += movementX*Vector2.Right*dt;
	}

	private void ApplyGravity(float dt)
	{
		_character.Velocity += Vector2.Down*_currentGravity*dt;
	}

	private void CapSpeed()
	{
		if (_character.Velocity.Length() > maxSpeed)
		{
			_character.Velocity = _character.Velocity.Normalized() * maxSpeed;
		}
	}

	private void SnapToGround()
	{
		if (_character.IsOnFloor() && _character.Velocity.Y > 0)
		{
			_character.Velocity = new Vector2(_character.Velocity.X, 0);
		}
	}
}
