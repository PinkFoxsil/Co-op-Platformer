using Godot;
using System;
using System.Collections.Generic;

public partial class DirectionalAttackComponent : Node2D, IComponent
{
    [Export] public float attackCooldown = 1f;
	[Export] public float attackDuration = 0.5f;
	[Export] public int attackDamage = 1;

	private float _attackCooldownTimer;

	private Hitbox _hitbox;

	private InputComponent _input;

    public Dictionary<CardinalDirection, Action> attacks = new()
    {
        [CardinalDirection.UP] = null,
        [CardinalDirection.DOWN] = null,
        [CardinalDirection.LEFT] = null,
        [CardinalDirection.RIGHT] = null
    };
	
	public void Init(Node parentNode)
	{
		Player _character = (Player) parentNode;
		_input = (InputComponent) _character.ComponentList.GetComponent(typeof(InputComponent));

		_hitbox = GetNode<Hitbox>("Hitbox");

        foreach (KeyValuePair<CardinalDirection, Action> attack in attacks)
        {
            if (attack.Value == null)
            {
                attacks[attack.Key] = GetAttack(attack.Key);
            }
        }

        attacks[CardinalDirection.DOWN] = null;
	}

    private Action GetAttack(CardinalDirection dir)
    {
        return () =>
        {
            RotateHitbox(dir);
            _hitbox.Activate(attackDuration);
        };
    }

	public void PhysicsProcess(float dt)
	{
		_attackCooldownTimer -= dt;

		if (_input == null)
		{
			return;
		}

		if (_input.attack1Pressed && CanAttack())
		{
            CardinalDirection dir = GetAttackDirection();
			Attack(dir);
		}
	}

	public virtual bool CanAttack()
	{
		return _attackCooldownTimer <= 0f;
	}

	private CardinalDirection GetAttackDirection()
	{
		return DirectionUtility.GetCardinalDirection(_input.mouseRelativePosition);
	}

    private void Attack(CardinalDirection dir)
    {
        if (attacks[dir] == null)
        {
            return;
        }

        _attackCooldownTimer = attackCooldown;
        attacks[dir]();
    }

	private void RotateHitbox(CardinalDirection dir)
	{
		Vector2 toVector = DirectionUtility.ToVector(dir);
		_hitbox.Rotation = toVector.Angle();
	}
}