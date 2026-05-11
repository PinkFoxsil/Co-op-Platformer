using Godot;
using System;

public partial class GroundSlamComponent : Node2D, IComponent
{
	[Export] public float attackCooldown = 1f;
	[Export] public float slamHitboxDuration = 0.5f;

	[Export] public int plungeAttackDamage = 1;
	[Export] public int slamAttackDamage = 1;

	private Hitbox _slamHitbox;
	private Hitbox _plungeHitbox;
	
	private Player _character;
	private InputComponent _input;
	private DirectionalAttackComponent _directionalAttack;
	
	public void Init(Node parentNode)
	{

		_character = (Player) parentNode;
		_input = (InputComponent) _character.ComponentList.GetComponent(typeof(InputComponent));
		_directionalAttack = (DirectionalAttackComponent) _character.ComponentList.GetComponent(typeof(DirectionalAttackComponent));

		_slamHitbox = GetNode<Hitbox>("SlamHitbox");
		_plungeHitbox = GetNode<Hitbox>("PlungeHitbox");

		_directionalAttack.attacks[CardinalDirection.DOWN] = Attack;
	}

	private void Attack()
	{
		if (_character.IsOnFloor())
		{
			_slamHitbox.Activate(slamHitboxDuration);
		}
	}
}
