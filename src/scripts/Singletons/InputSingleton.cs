using Godot;
using System;

public partial class InputSingleton : Node
{
    public float inputX;
	
	public bool jumpPressed;
	public bool jumpHeld;

	public bool ability1Pressed;
	public bool ability1Held;
	public bool ability1Released;

	public bool attack1Pressed;
	public bool attack1Held;
	public bool attack1Released;

	public bool attack2Pressed;
	public bool attack2Held;
	public bool attack2Released;

    public static InputSingleton Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

	public override void _PhysicsProcess(double dt)
	{
		inputX = Input.GetActionStrength("Right") - Input.GetActionStrength("Left");

		jumpPressed = Input.IsActionJustPressed("Jump");
		jumpHeld = Input.IsActionPressed("Jump");

		attack1Pressed = Input.IsActionJustPressed("Attack1");
		attack1Held = Input.IsActionPressed("Attack1");
		attack1Released = Input.IsActionJustReleased("Attack1");

		attack2Pressed = Input.IsActionJustPressed("Attack2");
		attack2Held = Input.IsActionPressed("Attack2");
		attack2Released = Input.IsActionJustReleased("Attack2");

		ability1Pressed = Input.IsActionJustPressed("Ability1");
		ability1Held = Input.IsActionPressed("Ability1");
		ability1Released = Input.IsActionJustReleased("Ability1");
	}
}