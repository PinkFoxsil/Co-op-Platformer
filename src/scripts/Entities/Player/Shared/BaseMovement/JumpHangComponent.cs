using Godot;

public partial class JumpHangComponent : Node, IActionComponent
{
    [Export] public float threshold = 32f;
    [Export] public float gravityMultiplier = 0.5f;

    private Player _player;
    private CharacterMotor _motor;

    public void Init(Node owner)
    {
        Player player = (Player) owner;
        _player = player;
        _motor = player.Motor;
    }

    public void PhysicsUpdate(float dt)
    {
        if (Mathf.Abs(_player.Velocity.Y) < threshold)
        {
            _motor.RequestGravityMultiplier(this, gravityMultiplier);
        }
    }
}