using Godot;
using System;

public partial class InputComponent : Component
{
    
    public float moveX;
    public bool jumpPressed;
    public bool dashPressed;

    public override void PrePhysicsProcess(float dt)
    {
        moveX = Input.GetActionStrength("Right") - Input.GetActionStrength("Left");
        jumpPressed = Input.IsActionJustPressed("Jump");
        //dashPressed = Input.IsActionJustPressed("dash");
    }
}
