using Godot;
using System;

public class InputMovementComponent : IComponent
{
    public float Deceleration = 100f;

    public float JumpForce = 100f;
    public float Gravity = 100f;
    public float FallMultiplier = 100f;

    public float CoyoteTime = 0.1f;
    public float CoyoteTimer;

    public float JumpBuffer = 0.1f;
    public float JumpBufferTimer;

    public bool IsGrounded;
}