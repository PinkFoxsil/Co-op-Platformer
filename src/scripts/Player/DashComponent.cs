using Godot;
using System;

public partial class DashComponent : Component
{
	[ExportCategory("Dash")]
 	[Export] public float dashSpeed = 1000f;
	[Export] public float dashTime = 0.05f;
	[Export] public float dashRecoveryTime = 0.15f;
	
	[Export] public float dashCooldown = 2f;
	[Export] public int maxDashes = 2;

	private int _currentDashCharges;
	private float _cooldownTimer;

	private float _dashRecoveryTimer;
	private bool _recovering;
	
	private float _dashTimer;
	private bool _dashing;

	private bool _requiresGroundReset;
	
	public override void PhysicsProcess(float dt)
	{
		InputComponent input = entity.GetComponent<InputComponent>();
		if (input == null) 
		{
			return;
		}
		
		UpdateCooldowns(dt);
		CheckDashTriggered(input);
		HandleGroundReset();
		UpdateDash(dt, input);
		UpdateRecovery(dt);
	}

	private void UpdateCooldowns(float dt)
	{
		if (_currentDashCharges == maxDashes)
		{
			return;
		}
		
		_cooldownTimer -= dt;
		if (_cooldownTimer > 0)
		{
			return;
		}
			
		_currentDashCharges++;
		if (_currentDashCharges < maxDashes)
		{
			_cooldownTimer = dashCooldown + _cooldownTimer;
		}
	}

    private void HandleGroundReset()
    {
        if (_requiresGroundReset && entity.IsOnFloor())
        {
            _requiresGroundReset = false;
        }
    }

	private void CheckDashTriggered(InputComponent input)
	{
		if (!input.ability1Pressed || input.lastInputX == 0)
		{
			return;
		}

		if (!CanDash())
		{
			return;
		}

		_dashing = true;
		_dashTimer = dashTime;

		_currentDashCharges--;

		if (_currentDashCharges == maxDashes - 1)
		{
			_cooldownTimer = dashCooldown;
		}

		if (_currentDashCharges == 0)
		{
			_requiresGroundReset = true;
		}
	}

	private bool CanDash()
	{
		return _currentDashCharges > 0 && !_dashing && !_recovering;
	}

	private void UpdateDash(float dt, InputComponent input)
	{
		if (!_dashing)
		{
			return;
		}
		
		ApplyDashForce(input.lastInputX);

		_dashTimer -= dt;
		if (_dashTimer <= 0)
		{
			_dashing = false;
			StartRecovery(dashRecoveryTime + _dashTimer);
		}
	}

	private void StartRecovery(float recoveryTime)
	{
		_recovering = true;
		_dashRecoveryTimer = recoveryTime;
	}

	private void ApplyDashForce(int direction)
	{
		entity.Velocity = new Vector2(direction * dashSpeed, 0);
	}

	private void UpdateRecovery(float dt)
	{
		if (!_recovering)
		{
			return;
		}

		_dashRecoveryTimer -= dt;
		if (_dashRecoveryTimer <= 0)
		{
			_recovering = false;
		}
	}
}
