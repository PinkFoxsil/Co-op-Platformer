using Godot;

public partial class PlayerInput : Node
{
    public InputSnapshot current { get; private set; }

    private Player _player;

    public void Init(Player player)
    {
        _player = player;
    }

    public void Capture()
    {
        Vector2 mouseWorld = _player.GetGlobalMousePosition();
        Vector2 mouseRelative = mouseWorld - _player.GlobalPosition;

        current = new InputSnapshot
        {
            
            // Move
            moveX = Input.GetAxis(
                "Left",
                "Right"
            ),

            // Jump
            jumpHeld = Input.IsActionJustPressed("Jump"),

            // Attack
            attack1Held = Input.IsActionPressed("Attack1"),

            // Dash
            dashHeld = Input.IsActionPressed("Ability1"),

            // Mouse
            mouseWorldPosition = mouseWorld,
            mouseRelativePosition = mouseRelative,
            mouseDirection = mouseRelative.Normalized()
        };
    }
}