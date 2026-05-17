using Godot;
using System;

public partial class BasicAttackComponent : Node2D, IActionComponent
{
    [ExportCategory("Attack")]
    [Export] public float attackCooldown = 1f;
    [Export] public float attackDuration = 0.5f;
    [Export] public int attackDamage = 1;

    [ExportCategory("Locks")]
    [Export] public bool lockMovementDuringAttack;
    [Export] public bool lockDashDuringAttack;

    private readonly Timer _cooldownTimer = new Timer();
    
    private bool _attackQueued;
    private bool _justAttacked;

    private CardinalDirection _attackDirection;
    private Hitbox _attackHitbox;
    
    private Player _player;
    private PlayerInput _input;
    private ActionOrchestrator _orchestrator;

    public void Init(Player player)
    {
        _player = player;
        _input = player.Input;
        _orchestrator = player.Orchestrator;

        Node2D hitboxes = GetNode<Node2D>("../Hitboxes");

        _attackHitbox = hitboxes.GetNode<Hitbox>("BasicAttackHitbox");
    }

    public void PrePhysicsUpdate(float dt)
    {
        _attackQueued = false;
        _justAttacked = false;

        if (!_input.current.attack1Held)
        {
            return;
        }

        if (!CanAttack())
        {
            return;
        }

        CardinalDirection direction = DirectionUtility.GetCardinalDirection(_input.current.mouseRelativePosition);
        if (direction == CardinalDirection.DOWN)
        {
            return;
        }

        _attackQueued = true;
        _attackDirection = direction;
    }

    public void PhysicsUpdate(float dt)
    {
        if (_attackQueued)
        {
            ExecuteAttack();
            _justAttacked = true;
        }
    }

    public void PostPhysicsUpdate(float dt)
    {   
        if (!_justAttacked)
        {
            UpdateCooldown(dt);  
        } 
    }

    public virtual bool CanAttack()
    {
        return !_cooldownTimer.IsRunning && _orchestrator.CanAttack();
    }

    protected virtual void ExecuteAttack()
    {
        _cooldownTimer.Start(attackCooldown);

        ApplyLocks();
        RotateHitbox(_attackDirection);

        _attackHitbox.Activate(attackDuration);
    }

    protected virtual void ApplyLocks()
    {
        _orchestrator.AddTag("AttackLocked");

        if (lockMovementDuringAttack)
        {
            _orchestrator.AddTag("MovementLocked");
        }

        if (lockDashDuringAttack)
        {
            _orchestrator.AddTag("DashLocked");
        }
    }

    protected virtual void RemoveLocks()
    {
        _orchestrator.RemoveTag("AttackLocked");

        if (lockMovementDuringAttack)
        {
            _orchestrator.RemoveTag("MovementLocked");
        }

        if (lockDashDuringAttack)
        {
            _orchestrator.RemoveTag("DashLocked");
        }
    }

    private void UpdateCooldown(float dt)
    {
        _cooldownTimer.Tick(dt);

        if (_cooldownTimer.HasStopped)
        {
            RemoveLocks();
        }
    }

    private void RotateHitbox(CardinalDirection direction)
    {
        Vector2 vector = DirectionUtility.ToVector(direction);
        _attackHitbox.Rotation = vector.Angle();
    }
}