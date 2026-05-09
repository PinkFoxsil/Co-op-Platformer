using Godot;
using System;

public partial class BaseMovementComponent : Component<Player>
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

    private Player _character;
    private InputComponent _input;

    public override void Init(Entity<Player> entity)
    {
        base.Init(entity);
        _character = entity.node;
        _input = entity.GetComponent<InputComponent>();
    }

    public override void PhysicsProcess(float dt)
    {
        if (_input == null)
        {
            GD.Print("Early return from no input");
            return;
        }
        
        UpdateCoyoteTime(dt);
        UpdateJumpBuffer(dt);
        ApplyHorizontalMovement(dt);
        CheckJump();
        ApplyGravity(dt);
        GroundSnap();
    }

    private void UpdateCoyoteTime(float dt)
    {
        if (_character.IsOnFloor())
        {
            _coyoteTimer = coyoteTime;
        } 
        else
        {
            _coyoteTimer -= dt;
        }
    }

    private void UpdateJumpBuffer(float dt)
    {
        if (_input.jumpPressed)
        {
            _jumpBufferTimer = jumpBuffer;
        }
        else
        {
            _jumpBufferTimer -= dt;
        }
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

        _coyoteTimer = 0;
        _jumpBufferTimer = 0;
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
        return _character.IsOnFloor() || (_coyoteTimer > 0 && _jumpBufferTimer > 0);
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
