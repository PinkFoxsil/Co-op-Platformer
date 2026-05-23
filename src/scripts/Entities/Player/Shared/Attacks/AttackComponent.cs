using Godot;
using System;

public partial class AttackComponent : Node, IActionComponent
{
	[ExportCategory("Attack")]
	[Export] public float attackCooldown = 1f;
	[Export] public float attackDuration = 0.5f;
	[Export] public int attackDamage = 1;

	[ExportCategory("Locks")]
	[Export] public bool lockMovementDuringAttack;
	[Export] public bool lockDashDuringAttack;

	protected readonly Timer _cooldownTimer = new Timer();
	
	protected bool _attackQueued;
	protected bool _justAttacked;
	
	protected Player _player;
	protected PlayerInput _input;
	protected ActionOrchestrator _orchestrator;

	public virtual void Init(Node owner)
	{
		_player = (Player) owner;
		_input = _player.Input;
		_orchestrator = _player.Orchestrator;
	}

	public virtual void PrePhysicsUpdate(float dt)
	{
		_attackQueued = false;
		_justAttacked = false;

		if (!AttackTriggered())
		{
			return;
		}

		if (!CanAttack())
		{
			return;
		}

		_attackQueued = true;
	}

	public virtual void PhysicsUpdate(float dt)
	{
		if (_attackQueued)
		{
			_cooldownTimer.Start(attackCooldown);
			ApplyLocks();
			ExecuteAttack();
			_justAttacked = true;
		}
	}

	public virtual void PostPhysicsUpdate(float dt)
	{   
		if (!_justAttacked)
		{
			UpdateCooldown(dt);  
		} 
	}

	public virtual bool AttackTriggered()
	{
		if (!_input.current.attack1Held)
		{
			return false;
		}
		
		return true;
	}

	public virtual bool CanAttack()
	{
		return !_cooldownTimer.IsRunning && _orchestrator.CanAttack();
	}

	protected virtual void ExecuteAttack(){}

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

	protected void UpdateCooldown(float dt)
	{
		_cooldownTimer.Tick(dt);

		if (_cooldownTimer.HasStopped)
		{
			RemoveLocks();
		}
	}
}
