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
    public MoveState State { get; private set; }

    private Player _player;

    public void Init(Node owner)
    {
        Player player = (Player) owner;
        _player = player;
    }

    public void PhysicsUpdate(float dt)
    {
        if (_player.IsOnFloor())
        {
            State = Mathf.IsZeroApprox(Mathf.Abs(_player.Velocity.X)) ? MoveState.Idle : MoveState.Running;
        }
        else
        {
            State = _player.Velocity.Y >= 0 ? MoveState.Falling : MoveState.Jumping;
        }
    }
}