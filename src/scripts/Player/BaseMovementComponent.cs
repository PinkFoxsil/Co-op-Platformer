using Godot;
using System;

public partial class BaseMovementComponent : Component
{
    [ExportCategory("Movement")]
    [Export] public float moveSpeed = 260f;
    [Export] public float acceleration = 2000f;
    [Export] public float deceleration = 3000f;

    [ExportCategory("Jumping")]
    [Export] public float jumpForce = 500f;

    [Export] public float coyoteTime = 0.125f;
    [Export] public float jumpBuffer = 0.1f;

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
    
    private float _coyoteTimer;
    private float _jumpBufferTimer;

    public override void PhysicsProcess(float dt)
    {
        InputComponent input = entity.GetComponent<InputComponent>();
        if (input == null)
        {
            return;
        }
        
        UpdateCoyoteTime(dt);
        UpdateJumpBuffer(input, dt);
        ApplyHorizontalMovement(input, dt);
        CheckJump(input);
        ApplyGravity(dt);
        GroundSnap();
    }

    private void UpdateCoyoteTime(float dt)
    {
        if (entity.IsOnFloor())
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

    private void ApplyHorizontalMovement(InputComponent input, float dt)
    {
        float control = entity.IsOnFloor() ? groundControl : airControl;
        float actualAcceleration = acceleration * control;
        float targetSpeed = input.inputX * moveSpeed;

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

    private void CheckJump(InputComponent input)
    {
        if (input.jumpPressed && CanJump())
        {
            Jump();
        }
    }

    private void Jump()
    {
        ApplyJumpForce();

        _coyoteTimer = 0;
        _jumpBufferTimer = 0;
    }

    private void ApplyJumpForce()
    {
        entity.Velocity = new Vector2(
            entity.Velocity.X,
            -jumpForce
        );
    }

    private bool CanJump()
    {
        return entity.IsOnFloor() || (_coyoteTimer > 0 && _jumpBufferTimer > 0);
    }

    private void ApplyGravity(float dt) // This function is lying and does more than one thing
    {
        Vector2 gravity = entity.GetGravity();

        // Make the player fall faster
        if (entity.Velocity.Y > 0)
        {
            gravity *= fallMultiplier;
        }

        // Adjust gravity at the peak of the player's jump
        if (Mathf.Abs(entity.Velocity.Y) < jumpHangTimeThreshold)
        {
            gravity *= jumpHangGravityMultiplier;
        }

        entity.Velocity += gravity * gravityScale * dt;

        // Cap max fall speed
        entity.Velocity = new Vector2(entity.Velocity.X, Mathf.Min(entity.Velocity.Y, maxFallSpeed));
    }

    private void GroundSnap()
    {
        if (entity.IsOnFloor() && entity.Velocity.Y > 0)
        {
            entity.Velocity = new Vector2(entity.Velocity.X, 0);
        }
    }
}
