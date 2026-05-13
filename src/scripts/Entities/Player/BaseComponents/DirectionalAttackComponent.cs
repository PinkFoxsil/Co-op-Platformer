using Godot;
using System;
using System.Collections.Generic;

public partial class DirectionalAttackComponent : Node2D, IComponent
{
    [Export] public float attackCooldown = 0.4f;
	[Export] public float hitboxDuration = 0.3f;
	[Export] public int damage = 1;

	public bool attackEnabled;
	public bool isAttacking;
	public CardinalDirection attackDirection;

	private Hitbox _hitbox;
	public Timer attackTimer;

	private Player _character;
	private InputSingleton _input;

    public Dictionary<CardinalDirection, Action> attacks = new()
    {
        [CardinalDirection.UP] = null,
        [CardinalDirection.DOWN] = null,
        [CardinalDirection.LEFT] = null,
        [CardinalDirection.RIGHT] = null
    };
	
	public void Init(Node parentNode)
	{
		_character = (Player) parentNode;
		_input = InputSingleton.Instance;

		_hitbox = GetNode<Hitbox>("Hitbox");
		attackTimer = new Timer();

        foreach (KeyValuePair<CardinalDirection, Action> attack in attacks)
        {
            if (attack.Value == null)
            {
                attacks[attack.Key] = GetAttack(attack.Key);
            }
        }
	}

    private Action GetAttack(CardinalDirection dir)
    {
        return () =>
        {
			attackTimer.Start(attackCooldown);
			isAttacking = true;

			RotateHitbox(dir);
            _hitbox.Activate(hitboxDuration);
        };
    }

	private void RotateHitbox(CardinalDirection dir)
	{
		Vector2 toVector = DirectionUtility.ToVector(dir);
		_hitbox.Rotation = toVector.Angle();
	}

	public void PhysicsProcess(float dt)
	{
		if (_input == null)
		{
			return;
		}
		
		if (_input.attack1Pressed && CanAttack())
		{
			Attack(attackDirection);
		}

		UpdateIsAttacking(dt);
	}

	private bool CanAttack()
	{
		return attackEnabled && !isAttacking;
	}

    public void Attack(CardinalDirection dir)
    {
        if (attacks[dir] == null)
        {
            return;
        }

        attacks[dir]();
    }

	private void UpdateIsAttacking(float dt)
	{
		attackTimer.Tick(dt);
		if (attackTimer.HasStopped)
		{
			isAttacking = false;
		}
	}
}