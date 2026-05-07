using Godot;
using System;

public partial class DashComponent : Component
{
	
 	[Export] public float dashSpeed = 400f;
	[Export] public float dashTime = 0.15f;
	[Export] public float dashRecoveryTime = 0.1f;
	
	[Export] public float dashCooldown = 2f;
	[Export] public int maxDashes = 2;

	private int _currentDashes;
	private float _cooldownTimer;

	private float _dashRecoveryTimer;
	private bool _recovering;
	
	private float _dashTimer;
	private bool _dashing;

	public override void _Ready()
	{
		_currentDashes = maxDashes;
	}
	
	public override void PhysicsProcess(float dt)
	{
		InputComponent input = entity.GetComponent<InputComponent>();
		if (input == null) 
		{
			return;
		}
		
		if (_currentDashes < maxDashes)
		{
			_cooldownTimer -= dt;

			if (_cooldownTimer <= 0)
			{
				_currentDashes++;
				if (_currentDashes < maxDashes) {
					_cooldownTimer = dashCooldown + _cooldownTimer;
				}
			}
		}

		float dir = Mathf.Sign(input.inputX);
		if (input.ability1Pressed && dir != 0 && _currentDashes > 0 && !_dashing && !_recovering)
		{
			_dashing = true;
			_dashTimer = dashTime;

			_currentDashes--;

			if (_currentDashes == maxDashes - 1)
			{
				_cooldownTimer = dashCooldown;
			}

			entity.Velocity = new Vector2(dir * dashSpeed, 0);
		}

		if (_dashing)
		{
			_dashTimer -= dt;

			entity.Velocity = new Vector2(
				Mathf.Sign(entity.Velocity.X) * dashSpeed,
				0
			);

			if (_dashTimer <= 0)
			{
				_dashing = false;
				_recovering = true;
				_dashRecoveryTimer = dashRecoveryTime + _dashTimer;
			}
		}
		
		if (_recovering)
		{
			_dashRecoveryTimer -= dt;

			if (_dashTimer <= 0)
			{
				_recovering = false;
			}
		}
	}
}
