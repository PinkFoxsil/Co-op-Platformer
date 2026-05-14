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
	[ExportCategory("Running")]
	[Export] public float moveSpeed = 350f;
	[Export] public float acceleration = 2000f;
	[Export] public float deceleration = 3000f;

	[ExportCategory("Jumping")]
	[Export] public float jumpHeight = 100f;
	[Export] public float jumpTimeToApex = 0.35f;
	[Export] public float fallMultiplier = 1.6f;
	[Export] public float maxFallSpeed = 1000f;

	[ExportGroup("Grace Timers")]
	[Export] public float coyoteTime = 0.125f;
	[Export] public float jumpBufferTime = 0.1f;

	[ExportGroup("Jump Hang")]
	[Export] public float jumpHangTimeThreshold = 32f;
	[Export] public float jumpHangGravityMultiplier = 0.5f;
	
	[ExportCategory("Movement Control")]
	[Export] public float airControl = 0.65f;
	[Export] public float groundControl = 1f;

	public bool movementEnabled;
	public MoveState moveState;

	private float _gravityStrength;
	private float _jumpForce;

	private Timer _coyoteTimer = new();
	private Timer _jumpBufferTimer = new();

	private Player _character;
	private InputSingleton _input;

	public void Init(Node parentNode)
	{
		_character = (Player) parentNode;
		_input = InputSingleton.Instance;

		moveState = _character.IsOnFloor() ? MoveState.Idle : MoveState.Falling;

		_gravityStrength = 2 * jumpHeight / (jumpTimeToApex * jumpTimeToApex);
		_jumpForce = _gravityStrength * jumpTimeToApex;
	}

	public void PrePhysicsProcess(float dt)
	{
		UpdateCoyoteTime(dt);
		UpdateJumpBuffer(dt);
	}

	public void PhysicsProcess(float dt)
	{
		float scaledGravity = _gravityStrength;
		float control = groundControl;
		float inputX = MathUtility.SnapToZero(_input.inputX);
		
		ProcessBufferedJump();

		if (moveState == MoveState.Falling)
		{
			scaledGravity = ApplyFallMultiplier(scaledGravity);
		}

		if (moveState == MoveState.Jumping || moveState == MoveState.Falling)
		{
			control = airControl;
			scaledGravity = ApplyJumpHangMultiplier(scaledGravity);
			ApplyGravity(dt, scaledGravity);
			ClampFallSpeed();
		}

		ApplyHorizontalMovement(dt, inputX, control);
		UpdateState();
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
		if (_input.jumpPressed) // jumpHeld auto jumps when landing which can feel bad when unintentional, I suggest adding a debounce before changing it back.
		{
			_jumpBufferTimer.Start(jumpBufferTime);
			return;
		}

		_jumpBufferTimer.Tick(dt);
	}

	private void ProcessBufferedJump()
	{
		if (_jumpBufferTimer.IsRunning)
		{
			GD.Print(_character.IsOnFloor());
			GD.Print(_coyoteTimer.IsRunning);
			GD.Print(" ");
		}
		
		if (_jumpBufferTimer.IsRunning && CanJump())
		{
			Jump();
		}
	}

	private bool CanJump()
	{
		return _character.IsOnFloor() || _coyoteTimer.IsRunning;
	}

	private void Jump()
	{
		_character.Velocity = new Vector2(_character.Velocity.X, -_jumpForce);

		_coyoteTimer.Stop();
		_jumpBufferTimer.Stop();
	}

	private float ApplyFallMultiplier(float scaledGravity)
	{
		return scaledGravity * fallMultiplier;
	}

	private float ApplyJumpHangMultiplier(float scaledGravity)
	{
		if (Mathf.Abs(_character.Velocity.Y) < jumpHangTimeThreshold)
		{
			return scaledGravity * jumpHangGravityMultiplier;
		}

		return scaledGravity;
	}

	private void ApplyGravity(float dt, float scaledGravity)
	{
		_character.Velocity += Vector2.Down * scaledGravity * dt;
	}

	private void ClampFallSpeed()
	{
		float clampedFallSpeed = Mathf.Min(_character.Velocity.Y, maxFallSpeed);
		_character.Velocity = new Vector2(_character.Velocity.X, clampedFallSpeed);
	}

	private void ApplyHorizontalMovement(float dt, float inputX, float control)
	{
		float targetSpeed = inputX * moveSpeed;
		float currentSpeed = MathUtility.SnapToZero(_character.Velocity.X);
		bool slowingDown = inputX == 0 || inputX * currentSpeed < 0;
		float rate = slowingDown ? deceleration : acceleration;
		
		rate *= control;

		float newVelocityX = Mathf.MoveToward(currentSpeed, targetSpeed, rate * dt);
		
		_character.Velocity = new Vector2(newVelocityX, _character.Velocity.Y);
	}

	private void UpdateState()
	{
		if (_character.IsOnFloor())
		{
			moveState = Mathf.Abs(_character.Velocity.X) < Mathf.Epsilon ? MoveState.Idle : MoveState.Running;
		}
		else
		{
			moveState = _character.Velocity.Y >= 0 ? MoveState.Falling : MoveState.Jumping;
		}
	}

}
