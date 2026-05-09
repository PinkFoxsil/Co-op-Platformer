using Godot;
using System;

public partial class InputSingleton
{
	public bool enabled;

	public float inputX;
	public int lastInputX;

	public Vector2 mouseWorldPosition;
	
	public bool jumpPressed;

	public bool ability1Pressed;
	public bool ability1Held;
	public bool ability1Released;

	public bool attack1Pressed;
	public bool attack1Held;
	public bool attack1Released;

	public bool attack2Pressed;
	public bool attack2Held;
	public bool attack2Released;

	public InputSingleton()
	{
		enabled = true;
	}

	public override void PrePhysicsProcess(float dt)
	{
		if (!enabled)
		{
			return;
		}

		inputX = Input.GetActionStrength("Right") - Input.GetActionStrength("Left");

		if (inputX != 0)
		{
			lastInputX = Mathf.Sign(inputX);
		}
		mouseWorldPosition = GetViewport().GetMousePosition();

		jumpPressed = Input.IsActionJustPressed("Jump");

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
