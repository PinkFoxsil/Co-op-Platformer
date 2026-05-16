using Godot;
public partial class JumpComponent : Node, IActionComponent
{
    [Export] public float jumpHeight = 112f;
    [Export] public float jumpTimeToApex = 0.3f;

    [Export] public float coyoteTime = 0.125f;
    [Export] public float jumpBufferTime = 0.1f;

    private float _gravity;
    private float _jumpForce;

    private Timer _coyoteTimer = new();
    private Timer _jumpBufferTimer = new();

    private bool _wasOnFloor;

    private Player _player;
    private CharacterMotor _motor;
    private PlayerInput _input;

    public void Init(Player player)
    {
        _player = player;
        _motor = player.Motor;
        _input = player.Input;

        _gravity = 2f * jumpHeight / (jumpTimeToApex * jumpTimeToApex);
        _jumpForce = _gravity * jumpTimeToApex;
    }

    public void PrePhysicsUpdate(float dt)
    {
        UpdateCoyoteTime(dt);
        UpdateJumpBuffer(dt);
    }

    public void PhysicsUpdate(float dt)
    {
        if (!_player.Orchestrator.CanMove())
        {
            return;
        }

        if (_jumpBufferTimer.IsRunning && CanJump())
        {
            Jump();
        }
    }

    private void Jump()
    {
        _motor.RequestVelocity(this, new Vector2(0, -_jumpForce), priority: 0);

        _coyoteTimer.Stop();
        _jumpBufferTimer.Stop();
    }

    private bool CanJump()
    {
        return _player.IsOnFloor() || _coyoteTimer.IsRunning;
    }

    private void UpdateCoyoteTime(float dt)
    {
        bool isOnFloor = _player.IsOnFloor();

        if (_wasOnFloor && !isOnFloor)
        {
            _coyoteTimer.Start(coyoteTime);
        }

        _wasOnFloor = isOnFloor;

        _coyoteTimer.Tick(dt);
    }

    private void UpdateJumpBuffer(float dt)
    {
        if (_input.current.jumpHeld)
        {
            _jumpBufferTimer.Start(jumpBufferTime);
        }

        _jumpBufferTimer.Tick(dt);
    }
}
