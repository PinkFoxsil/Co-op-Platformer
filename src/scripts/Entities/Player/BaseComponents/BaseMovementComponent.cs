using Godot;
using System;

public partial class BaseMovementComponent : Node, IComponent
{
	[ExportCategory("Movement")]
	[Export] public float moveSpeed = 260f;
	[Export] public float acceleration = 2000f;
	[Export] public float deceleration = 3000f;

	[ExportCategory("Jumping")]
	[Export] public float jumpForce = 500f;

	[Export] public float coyoteTime = 0.125f;
	[Export] public float jumpBufferTime = 0.1f;

	[ExportGroup("Gravity")]
	[Export] public float gravityScale = 1.4f;
	[Export] public float fallMultiplier = 1.4f;
	[Export] public float maxFallSpeed = 1000f;

	[ExportGroup("Jump Hang")]
	[Export] public float jumpHangTimeThreshold = 5f;
	[Export] public float jumpHangGravityMultiplier = 0.5f;

	[ExportCategory("Control")]
	[Export] public float airControl = 0.6f;
	[Export] public float groundControl = 1f;
	
	private Timer _coyoteTimer = new Timer();
	private Timer _jumpBufferTimer = new Timer();

	private Player _character;
	private InputComponent _input;

	public void Init(Node parentNode)
	{
		_character = (Player) parentNode;
		_input = (InputComponent) _character.ComponentList.GetComponent(typeof(InputComponent));
	}

	public void PrePhysicsProcess(float dt)
	{
		UpdateCoyoteTime(dt);
		UpdateJumpBuffer(dt);
	}

	public void PhysicsProcess(float dt)
	{
		ApplyHorizontalMovement(dt);
		CheckJump();
		ApplyGravity(dt);
		GroundSnap();
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

	private void ApplyHorizontalMovement(float dt)
	{
		
		float control = _character.IsOnFloor() ? groundControl : airControl;
		float actualAcceleration = acceleration * control;
		float targetSpeed = _input.inputX * moveSpeed;

		if (Mathf.Abs(targetSpeed) > 0.01f)
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

	private void CheckJump()
	{
		if (_input.jumpPressed && CanJump())
		{
			Jump();
		}
	}

	private void Jump()
	{
		ApplyJumpForce();

		_coyoteTimer.Stop();
		_jumpBufferTimer.Stop();
	}

	private void ApplyJumpForce()
	{
		_character.Velocity = new Vector2(
			_character.Velocity.X,
			-jumpForce
		);
	}

	private bool CanJump()
	{
		return _character.IsOnFloor() || (_coyoteTimer.isRunning && _jumpBufferTimer.isRunning);
	}

	private void ApplyGravity(float dt) // This function is lying and does more than one thing
	{
		Vector2 gravity = _character.GetGravity();

		// Make the player fall faster
		if (_character.Velocity.Y > 0)
		{
			gravity *= fallMultiplier;
		}

		// Adjust gravity at the peak of the player's jump
		if (Mathf.Abs(_character.Velocity.Y) < jumpHangTimeThreshold)
		{
			gravity *= jumpHangGravityMultiplier;
		}

		_character.Velocity += gravity * gravityScale * dt;

		// Cap max fall speed
		_character.Velocity = new Vector2(_character.Velocity.X, Mathf.Min(_character.Velocity.Y, maxFallSpeed));
	}

	private void GroundSnap()
	{
		if (_character.IsOnFloor() && _character.Velocity.Y > 0)
		{
			_character.Velocity = new Vector2(_character.Velocity.X, 0);
		}
	}
}
