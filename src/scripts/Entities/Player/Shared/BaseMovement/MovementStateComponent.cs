using Godot;

public enum MoveState
{
    Idle,
    Running,
    Jumping,
    Falling
}

public partial class MovementStateComponent : Node, IActionComponent
{
    public MoveState state { get; private set; }

    private Player _player;

    public void Init(Player player)
    {
        _player = player;
    }

    public void PhysicsUpdate(float dt)
    {
        if (_player.IsOnFloor())
        {
            state = Mathf.IsZeroApprox(Mathf.Abs(_player.Velocity.X)) ? MoveState.Idle: MoveState.Running;
        }
        else
        {
            state = _player.Velocity.Y >= 0 ? MoveState.Falling : MoveState.Jumping;
        }
    }
}