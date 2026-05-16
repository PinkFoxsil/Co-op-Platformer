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
                "move_left",
                "move_right"
            ),

            // Jump
            jumpHeld = Input.IsActionPressed("jump"),

            // Attack
            attack1Held = Input.IsActionPressed("attack"),

            // Dash
            dashHeld = Input.IsActionPressed("dash"),

            // Mouse
            mouseWorldPosition = mouseWorld,
            mouseRelativePosition = mouseRelative,
            mouseDirection = mouseRelative.Normalized()
        };
    }
}