using Godot;

public partial class JumpHangComponent : Node, IActionComponent
{
    [Export] public float threshold = 32f;
    [Export] public float gravityMultiplier = 0.5f;

    private Player _player;
    private CharacterMotor _motor;
    private PlayableCharacterData _characterData;

    public void Init(Node owner)
    {
        Player player = (Player) owner;
        _player = player;
        _motor = player.Motor;

        _characterData = owner.GetNode<PlayableCharacterData>("CharacterData");
    }

    public void PhysicsUpdate(float dt)
    {
        if (Mathf.Abs(_player.Velocity.Y) < threshold)
        {
            _motor.RequestVelocity(this, new Vector2(0, _characterData.Gravity * -0.5f) * dt);
        }
    }
}