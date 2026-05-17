using Godot;

public enum DashState
{
    Idle,
    Dashing,
    Recovering,
}

public partial class DashComponent : Node, IActionComponent
{
    [ExportCategory("Dash")]
    [Export] public float dashSpeed = 1000f;
    [Export] public float dashTime = 0.05f;
    [Export] public float dashRecoveryTime = 0.15f;
    [Export] public float dashCooldown = 2f;
    [Export] public int maxDashes = 2;

    public bool dashEnabled = true;
    public DashState dashState;

    private int _currentDashCharges;
    private int _dashDirection;
    private int _lastFacedDirection;

    private bool _justDashed;
    private bool _requiresGroundReset;

    private Timer _dashTimer = new();
    private Timer _dashRecoveryTimer = new();
    private Timer _cooldownTimer = new();

    private Player _player;
    private PlayerInput _input;
    private CharacterMotor _motor;
    private ActionOrchestrator _orchestrator;

    public void Init(Node owner)
    {
        Player player = (Player) owner;
        _player = player;
        _input = player.Input;
        _motor = player.Motor;
        _orchestrator = player.Orchestrator;

        _lastFacedDirection = 1;
        _currentDashCharges = maxDashes;
    }

    public void PrePhysicsUpdate(float dt)
    {
        _justDashed = false;

        if (dashState == DashState.Recovering)
        {
            UpdateRecovery(dt);
        }

        UpdateCooldowns(dt);

        UpdateLastFacedDirection();
        if (dashState == DashState.Idle)
        {
            _justDashed = CheckDashTriggered();
        }
    }

    public void PhysicsUpdate(float dt)
    {
        if (dashState == DashState.Dashing)
        {
            ApplyDashForce();
        }
    }

    public void PostPhysicsUpdate(float dt)
    {
        HandleGroundReset();
        if (!_justDashed && dashState == DashState.Dashing)
        {
            UpdateDashDuration(dt);
        }
    }

    private void UpdateLastFacedDirection()
    {
        if (!Mathf.IsZeroApprox(_input.current.moveX))
        {
            _lastFacedDirection = _input.current.moveX >= 0f ? 1 : -1;
        }
    }

    private void UpdateDashDuration(float dt)
    {
        float excess = _dashTimer.Tick(dt);

        if (_dashTimer.HasStopped)
        {
            dashState = DashState.Recovering;

            _dashRecoveryTimer.Start(dashRecoveryTime - excess);

            _orchestrator.RemoveTag("MovementLocked");
        }
    }

    private void UpdateRecovery(float dt)
    {
        _dashRecoveryTimer.Tick(dt);

        if (_dashRecoveryTimer.HasStopped)
        {
            dashState = DashState.Idle;
        }
    }

    private void UpdateCooldowns(float dt)
    {
        if (_currentDashCharges == maxDashes)
        {
            return;
        }

        float excess = _cooldownTimer.Tick(dt);
        if (_cooldownTimer.IsRunning)
        {
            return;
        }

        _currentDashCharges++;
        if (_currentDashCharges < maxDashes)
        {
            _cooldownTimer.Start(dashCooldown - excess);
        }
    }

    private bool CanDash()
    {
        return dashEnabled && _currentDashCharges > 0 && _orchestrator.CanDash();
    }

    private bool CheckDashTriggered()
    {
        if (!_input.current.dashHeld)
        {
            return false;
        }

        if (!CanDash())
        {
            return false;
        }

        dashState = DashState.Dashing;

        _dashTimer.Start(dashTime);
        _dashDirection = _lastFacedDirection;
        _currentDashCharges--;

        _orchestrator.AddTag("MovementLocked");

        if (_currentDashCharges == maxDashes - 1)
        {
            _cooldownTimer.Start(dashCooldown);
        }

        if (_currentDashCharges == 0)
        {
            _requiresGroundReset = true;
        }

        return true;
    }

    private void ApplyDashForce()
    {
        _motor.RequestVelocity(this, new Vector2(_dashDirection * dashSpeed, 0), priority: 10);
        _motor.RequestGravityMultiplier(this, 0f, priority: 10);
    }

    private void HandleGroundReset()
    {
        if (_requiresGroundReset && _player.IsOnFloor())
        {
            _requiresGroundReset = false;
        }
    }
}