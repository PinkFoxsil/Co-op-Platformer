using Godot;
using System;

public partial class DashComponent : Node, IComponent
{
	[ExportCategory("Dash")]
 	[Export] public float dashSpeed = 1000f;
	[Export] public float dashTime = 0.05f;
	[Export] public float dashRecoveryTime = 0.15f;
	[Export] public float dashCooldown = 2f;
	[Export] public int maxDashes = 2;

	public bool canDash;
	public bool isDashing;
	public int dashDirection;

	private int _currentDashCharges;
	private bool _requiresGroundReset;
	private bool _justDashed;

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
		UpdateCooldowns(dt);
		_justDashed = CheckDashTriggered();	
		UpdateRecovery(dt);
	}

	public void PhysicsProcess(float dt)
	{
		UpdateDash(dt);
	}

	public void PostPhysicsProcess(float dt)
	{
		HandleGroundReset();
		if (!_justDashed)
		{
			UpdateDashDuration(dt);
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

	private void UpdateDashDuration(float dt)
	{
		float excess = _dashTimer.Tick(dt);
		if (_dashTimer.IsRunning)
		{
			StartRecovery(dashRecoveryTime - excess);
		}
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

		_dashTimer.Start(dashTime);
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

	private bool CanDash()
	{
		return _currentDashCharges > 0 && !_dashTimer.IsRunning && !_dashRecoveryTimer.IsRunning;
	}

	private void HandleGroundReset()
	{
		if (_requiresGroundReset && _character.IsOnFloor())
		{
			_requiresGroundReset = false;
		}
	}

	private void UpdateDash(float dt)
	{
		if (!_dashTimer.IsRunning)
		{
			return;
		}
		
		ApplyDashForce(dashDirection);
	}

	private void StartRecovery(float recoveryTime)
	{
		_dashRecoveryTimer.Start(recoveryTime);
	}

	private void ApplyDashForce(int directionX)
	{
		if (directionX == 0)
		{
			return;
		}

		_character.Velocity = new Vector2(directionX * dashSpeed, 0);
	}

	private void UpdateRecovery(float dt)
	{
		if (!_dashRecoveryTimer.IsRunning)
		{
			return;
		}

		_dashRecoveryTimer.Tick(dt);
	}
}
