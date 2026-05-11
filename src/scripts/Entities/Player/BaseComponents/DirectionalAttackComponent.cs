using Godot;
using System;
using System.Collections.Generic;

public partial class DirectionalAttackComponent : Node2D, IComponent
{
    [Export] public float attackCooldown = 1f;
	[Export] public float hitboxDuration = 0.5f;
	[Export] public int damage = 1;

	private Hitbox _hitbox;

	private InputComponent _input;
	private Player _character;

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
		_input = (InputComponent) _character.ComponentList.GetComponent(typeof(InputComponent));

		_hitbox = GetNode<Hitbox>("Hitbox");

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
			_character.currentState.stateTag = PlayerStateTag.Attacking;
            _character.currentState.Start(attackCooldown);
			
            RotateHitbox(dir);
            _hitbox.Activate(hitboxDuration);
        };
    }

	public void PhysicsProcess(float dt)
	{
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
		return _character.currentState.stateTag == PlayerStateTag.Neutral;
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

        attacks[dir]();
    }

	private void RotateHitbox(CardinalDirection dir)
	{
		Vector2 toVector = DirectionUtility.ToVector(dir);
		_hitbox.Rotation = toVector.Angle();
	}
}