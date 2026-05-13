using Godot;
using System;

public enum DashState
{
	Idle,
	Dashing,
	Recovering,
}

public partial class DashComponent : Node, IComponent
{
	[ExportCategory("Dash")]
 	[Export] public float dashSpeed = 1000f;
	[Export] public float dashTime = 0.05f;
	[Export] public float dashRecoveryTime = 0.15f;
	[Export] public float dashCooldown = 2f;
	[Export] public int maxDashes = 2;

	public bool dashEnabled;
	public DashState dashState;

	private int _currentDashCharges;
	private int _dashDirection;
	private bool _justDashed;
	private bool _requiresGroundReset;

	private Timer _dashTimer = new();
	private Timer _dashRecoveryTimer = new();
	private Timer _cooldownTimer = new();

	private Player _character;
	private InputSingleton _input;

	public void Init(Node parentNode)
	{
		_character = (Player) parentNode;
		_input = InputSingleton.Instance;
	}

	public void PrePhysicsProcess(float dt)
	{
		UpdateRecovery(dt);
		UpdateCooldowns(dt);
		_justDashed = CheckDashTriggered();	
	}

	public void PhysicsProcess(float dt)
	{
		ApplyDashForce(dt);
	}

	public void PostPhysicsProcess(float dt)
	{
		HandleGroundReset();
		if (!_justDashed)
		{
			UpdateDashDuration(dt);
		}
	}

	private void UpdateDashDuration(float dt)
	{

		if (!_dashTimer.IsRunning)
		{
			return;
		}

		float excess = _dashTimer.Tick(dt);
		if (_dashTimer.HasStopped)
		{
			dashState = DashState.Recovering;
			_dashRecoveryTimer.Start(dashRecoveryTime - excess);
		}
	}

	private void UpdateRecovery(float dt)
	{
		if (!_dashRecoveryTimer.IsRunning)
		{
			return;
		}

		_dashRecoveryTimer.Tick(dt);

		if (_dashTimer.HasStopped)
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
		return dashEnabled && _currentDashCharges > 0 && !_dashTimer.IsRunning && !_dashRecoveryTimer.IsRunning;
	}

	private bool CheckDashTriggered()
	{
		if (!_input.ability1Pressed)
		{
			return false;
		}

		if (!CanDash())
		{
			return false;
		}

		dashState = DashState.Dashing;

		_dashTimer.Start(dashTime);
		_dashDirection = _input.inputX > 0 ? 1 : -1;
		_currentDashCharges--;

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

	private void ApplyDashForce(float dt)
	{
		if (!_dashTimer.IsRunning)
		{
			return;
		}
		
		_character.Velocity = new Vector2(_dashDirection * dashSpeed, 0);
	}

	private void HandleGroundReset()
	{
		if (_requiresGroundReset && _character.IsOnFloor())
		{
			_requiresGroundReset = false;
		}
	}
}
