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
	
	private Player _character;
    private InputComponent _input;


    public override void Init(Node parentNode)
	{
		base.Init(parentNode);

		_character = (Player) parentNode;
		_input = (InputComponent) _character.ComponentList.GetComponent(typeof(InputComponent));
    }

	public override void PhysicsProcess(float dt)
	{
		if (_input == null) 
		{
			return;
		}
		
		UpdateCooldowns(dt);
		CheckDashTriggered();
		HandleGroundReset();
		UpdateDash(dt);
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

	private void CheckDashTriggered()
	{
		if (!_input.ability1Pressed)
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

	private void HandleGroundReset()
    {
        if (_requiresGroundReset && _character.IsOnFloor())
        {
            _requiresGroundReset = false;
        }
    }

	private void UpdateDash(float dt)
	{
		if (!_dashing)
		{
			return;
		}
		
		ApplyDashForce(_input.lastInputX);

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

	private void ApplyDashForce(int directionX)
	{
		if (directionX == 0)
		{
			directionX = 1;
		}

		_character.Velocity = new Vector2(directionX * dashSpeed, 0);
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