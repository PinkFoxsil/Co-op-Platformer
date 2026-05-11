using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private ComponentList _componentList;

	public ComponentList ComponentList => _componentList;

	private Timer _attackLockTimer = new();
	private Timer _movementLockTimer = new();

	public bool attackLocked => _attackLockTimer.isRunning;
	public bool movementLocked => _movementLockTimer.isRunning;

	public override void _Ready()
	{
		_componentList = new ComponentList(this);
		_componentList.RegisterChildren();
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float) delta;
		
		UpdateTimers(dt);

		// [TBD] Determine ordering of processing
		_componentList.PrePhysicsProcess(dt);
		_componentList.PhysicsProcess(dt);

		MoveAndSlide();

		_componentList.PostPhysicsProcess(dt);
	}

	private void UpdateTimers(float dt)
	{
		_attackLockTimer.Tick(dt);
		_movementLockTimer.Tick(dt);
	}

	public void LockAttacks()
	{
		_attackLockTimer.Start();
	}

	public void LockAttacks(float duration)
	{
		_attackLockTimer.Start(duration);
	}

	public void UnlockAttacks()
	{
		_attackLockTimer.Stop();
	}

	public void LockMovement()
	{
		_movementLockTimer.Start();
	}

	public void LockMovement(float duration)
	{
		_movementLockTimer.Start(duration);
	}

	public void UnlockMovement()
	{
		_movementLockTimer.Stop();
	}

}