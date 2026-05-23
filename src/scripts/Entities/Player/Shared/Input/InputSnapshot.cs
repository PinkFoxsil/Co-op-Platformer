using Godot;

public struct InputSnapshot
{

	// Movement
	public float moveX;

	// Jump
	public bool jumpHeld;

	// Attack
	public bool attack1Held;

	// Dash
	public bool dashHeld;
	public bool dashReleased;

	public Vector2 mouseWorldPosition;
	public Vector2 mouseRelativePosition;
	public Vector2 mouseDirection;
}
