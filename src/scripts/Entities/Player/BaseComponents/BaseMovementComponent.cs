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
	[Export] public float moveSpeed = 270f;
	[Export] public float acceleration = 2000f;
	[Export] public float deceleration = 3000f;

	[ExportCategory("Jumping")]
	[Export] public float jumpForce = 560f;

	[Export] public float coyoteTime = 0.125f;
	[Export] public float jumpBufferTime = 0.1f;

	[ExportGroup("Gravity")]
	[Export] public float gravityScale = 1.4f;
	[Export] public float fallMultiplier = 1.4f;
	[Export] public float maxSpeed = 1000f;

	[ExportGroup("Jump Hang")]
	[Export] public float jumpHangTimeThreshold = 5f;
	[Export] public float jumpHangGravityMultiplier = 0.5f;

	[ExportCategory("Control")]
	[Export] public float airControl = 0.6f;
	[Export] public float groundControl = 1f;

	public bool canMove;
	public MoveState moveState;
	
	private Vector2 _currentGravity;

	private Timer _coyoteTimer = new Timer();
	private Timer _jumpBufferTimer = new Timer();

	private Player _character;
	private InputSingleton _input;

	public void Init(Node parentNode)
	{
		_character = (Player) parentNode;
		_input = InputSingleton.Instance;

		moveState = _character.IsOnFloor() ? MoveState.Idle : MoveState.Falling;
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
		_currentGravity = _character.GetGravity() * gravityScale;

		ApplyHorizontalMovement(dt);

		UpdateState();

		if (moveState == MoveState.Idle ||
			moveState == MoveState.Running)
		{
			CheckJump();
		}

		if (moveState == MoveState.Jumping ||
			moveState == MoveState.Falling)
		{
			AdjustHangTimeGravity();
		}
		
		if (moveState == MoveState.Falling)
		{
			ApplyFallMultiplier();
		}
		
		ApplyGravity(dt);
		CapSpeed();
		GroundSnap();
	}

	private void ApplyHorizontalMovement(float dt)
	{
		float control = _character.IsOnFloor() ? groundControl : airControl;
		float actualAcceleration = acceleration * control;
		float targetSpeed = _input.inputX * moveSpeed;

		if (Mathf.Abs(targetSpeed) > Mathf.Epsilon)
		{
			_character.Velocity = new Vector2(
				Mathf.MoveToward(_character.Velocity.X, targetSpeed, actualAcceleration * dt),
				_character.Velocity.Y
			);
		}
		else
		{
			_character.Velocity = new Vector2(
				Mathf.MoveToward(_character.Velocity.X, 0, deceleration * dt * control),
				_character.Velocity.Y
			);
		}
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
		if (_input.jumpPressed && canMove)
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
			-jumpForce
		);
	}

	// Adjust gravity at the peak of the player's jump
	private void AdjustHangTimeGravity()
	{
		if (Mathf.Abs(_character.Velocity.Y) < jumpHangTimeThreshold)
		{
			_currentGravity *= jumpHangGravityMultiplier;
		}
	}

	private void ApplyFallMultiplier()
	{
		_currentGravity *= fallMultiplier;
	}

	private void ApplyGravity(float dt)
	{
		_character.Velocity += _currentGravity * dt;
	}

	private void CapSpeed()
	{
		if (_character.Velocity.Length() > maxSpeed)
		{
			_character.Velocity = _character.Velocity.Normalized() * maxSpeed;
		}
	}

	private void GroundSnap()
	{
		if (_character.IsOnFloor() && _character.Velocity.Y > 0)
		{
			_character.Velocity = new Vector2(_character.Velocity.X, 0);
		}
	}
}
