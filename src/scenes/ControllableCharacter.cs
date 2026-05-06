using Godot;
using System;

public partial class ControllableCharacter : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity = applyGravity((float)delta, velocity);
		}

		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
		{
			velocity = applyJump(velocity);
		}

		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
		velocity = applyMovement(direction, velocity);

		Velocity = velocity;
		MoveAndSlide();
	}

	private Vector2 applyGravity(float delta, Vector2 velocity)
	{
		if (velocity.Y > 0)
		{
			velocity += GetGravity() * 1.5f * delta;
		}
		else
		{
			velocity += GetGravity() * delta;
		}
		
		return velocity;
	}

	private Vector2 applyJump(Vector2 velocity)
	{
		velocity.Y = JumpVelocity;
		return velocity;
	}

	private Vector2 applyMovement(Vector2 direction, Vector2 velocity)
	{
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = 0;
		}

		return velocity;
	}
}