using Godot;
using System;

public partial class InputComponent : Component
{
    
    public float moveX;
    public bool jumpPressed;
    public bool dashPressed;

    public override void PrePhysicsProcess(float dt)
    {
        moveX = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        jumpPressed = Input.IsActionJustPressed("jump");
        dashPressed = Input.IsActionJustPressed("dash");
    }
}
