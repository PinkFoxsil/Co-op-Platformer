using Godot;
using System;

public partial class MovementComponent : Component
{
    [Export] public float moveSpeed = 300f;
    [Export] public float acceleration = 2000f;
    [Export] public float deceleration = 2000f;

    [Export] public float jumpForce = 400f;
    [Export] public float gravityScale = 1.4f;
    [Export] public float fallMultiplier = 1.6f;
    [Export] public float maxFallSpeed = 1000f;

    [Export] public float coyoteTime = 0.12f;
    [Export] public float jumpBuffer = 0.1f;
    [Export] public float jumpHangTimeThreshold = 5f;
    [Export] public float jumpHangGravityMultiplier = 0.5f;

    [Export] public float airControl = 0.7f;
    [Export] public float groundControl = 1f;
    
    private float _coyoteTimer;
    private float _jumpBufferTimer;

    public override void PhysicsProcess(float dt)
    {
        var input = entity.GetComponent<InputComponent>();
        if (input == null)
        {
            return;
        }

        bool grounded = entity.IsOnFloor();
        
        UpdateCoyoteTime(grounded, dt);
        UpdateJumpBuffer(input, dt);
        ApplyHorizontalMovement(grounded, input, dt);
        ApplyJump();
        ApplyGravity(dt);
        GroundSnap(grounded);
    }

    private void UpdateCoyoteTime(bool isGrounded, float dt)
    {
        if (isGrounded)
        {
            _coyoteTimer = coyoteTime;
        } 
        else
        {
            _coyoteTimer -= dt;
        }
    }

    private void UpdateJumpBuffer(InputComponent input, float dt)
    {
        if (input.jumpPressed)
        {
            _jumpBufferTimer = jumpBuffer;
        }
        else
        {
            _jumpBufferTimer -= dt;
        }
    }

    private void ApplyHorizontalMovement(bool grounded, InputComponent input, float dt)
    {
        float control = grounded ? groundControl : airControl;
        float actualAcceleration = acceleration * control;
        float targetSpeed = input.moveX * moveSpeed;

        if (Mathf.Abs(targetSpeed) > 0.01f)
        {
            entity.Velocity = new Vector2(
                Mathf.MoveToward(entity.Velocity.X, targetSpeed, actualAcceleration * dt),
                entity.Velocity.Y
            );
        }
        else
        {
            entity.Velocity = new Vector2(
                Mathf.MoveToward(entity.Velocity.X, 0, deceleration * dt * control),
                entity.Velocity.Y
            );
        }
    }

    private void ApplyJump()
    {
        if (_coyoteTimer > 0 && _jumpBufferTimer > 0)
        {
            entity.Velocity = new Vector2(
                entity.Velocity.X,
                -jumpForce
            );

            _coyoteTimer = 0;
            _jumpBufferTimer = 0;
        }
    }

    private void ApplyGravity(float dt)
    {
        Vector2 gravity = entity.GetGravity();

        if (entity.Velocity.Y > 0)
        {
            gravity *= fallMultiplier;
        }

        if (Mathf.Abs(entity.Velocity.Y) < jumpHangTimeThreshold)
        {
            gravity *= jumpHangGravityMultiplier;
        }

        entity.Velocity += gravity * gravityScale * dt;

        // Cap max fall speed
        entity.Velocity = new Vector2(entity.Velocity.X, Mathf.Min(entity.Velocity.Y, maxFallSpeed));
    }

    private void GroundSnap(bool grounded)
    {
        if (grounded && entity.Velocity.Y > 0)
        {
            entity.Velocity = new Vector2(entity.Velocity.X, 0);
        }
    }
}
