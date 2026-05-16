using Godot;
using System;

public partial class GroundSlamComponent : Node2D, IComponent
{
	[Export] public float plungeForce = 1750f;
	[Export] public int plungeAttackDamage = 1;

	[Export] public float slamCooldown = 0.4f;
	[Export] public float slamHitboxDuration = 0.3f;
	[Export] public int slamAttackDamage = 1;

	public bool canSlam;
	public bool isPlunging;

	private Hitbox _slamHitbox;
	private Hitbox _plungeHitbox;
	
	private Player _character;
	private InputSingleton _input;
	private DirectionalAttackComponent _directionalAttack;
	
	public void Init(Node parentNode)
	{

		_character = (Player) parentNode;
		_input = InputSingleton.Instance;
		_directionalAttack = (DirectionalAttackComponent) _character.ComponentList.GetComponent(typeof(DirectionalAttackComponent));

		_slamHitbox = GetNode<Hitbox>("SlamHitbox");
		_plungeHitbox = GetNode<Hitbox>("PlungeHitbox");

		_directionalAttack.attacks[CardinalDirection.DOWN] = Attack;
	}

	public void PhysicsProcess(float dt)
	{
		if (isPlunging) {
			if (_character.IsOnFloor())
			{
				isPlunging = false;
				_plungeHitbox.Deactivate();
				_directionalAttack.Attack(CardinalDirection.DOWN);
			}
			else
			{
				ApplyDownwardsVelocity();
			}
		}
	}

	private void Attack()
	{
		if (_character.IsOnFloor())
		{
			_directionalAttack.attackTimer.Start(slamCooldown);
			_directionalAttack.isAttacking = true;

			_slamHitbox.Activate(slamHitboxDuration);
		}
		else
		{
			isPlunging = true;
			ApplyDownwardsVelocity();
			_plungeHitbox.Activate();
		}
	}

	private void ApplyDownwardsVelocity()
	{
		_character.Velocity = new Vector2(_character.Velocity.X, plungeForce);
	}
}
