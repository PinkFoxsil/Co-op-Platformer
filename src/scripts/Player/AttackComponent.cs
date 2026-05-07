using Godot;
using System;

public partial class AttackComponent : Component
{
    [Export] public float attackCooldown = 1f;
    [Export] public float attackDuration = 0.5f;
    [Export] public int attackDamage = 1;

    [Export] bool collisionVisible = false;

    private float _attackCooldownTimer;

    private Area2D _leftAttackHitbox;
    private Area2D _rightAttackHitbox;
    private Area2D _upAttackHitbox;

    private float _leftHitboxTimer;
    private float _rightHitboxTimer;
    private float _upHitboxTimer;

    public override void _Ready()
    {
        _attackCooldownTimer = 0f;
        
        _leftHitboxTimer = 0f;
        _rightHitboxTimer = 0f;
        _upHitboxTimer = 0f;

        _leftAttackHitbox = GetNode<Area2D>("Left");
        _rightAttackHitbox = GetNode<Area2D>("Right");
        _upAttackHitbox = GetNode<Area2D>("Up");
    }

    public override void PhysicsProcess(float dt)
    {
        _attackCooldownTimer -= dt;

        if (_leftHitboxTimer <= 0f)
        {
            _leftAttackHitbox.GetNode<CollisionShape2D>("Hitbox").Disabled = true;
        }
        else
        {
            _leftHitboxTimer -= dt;
        }

        if (_rightHitboxTimer <= 0f)
        {
            _rightAttackHitbox.GetNode<CollisionShape2D>("Hitbox").Disabled = true;
        }
        else
        {
            _rightHitboxTimer -= dt;
        }

        if (_upHitboxTimer <= 0f)
        {
            _upAttackHitbox.GetNode<CollisionShape2D>("Hitbox").Disabled = true;
        }
        else
        {
            _upHitboxTimer -= dt;
        }

        InputComponent input = entity.GetComponent<InputComponent>();
        if (input == null)
        {
            return;
        }

        if (input.attack1Pressed && _attackCooldownTimer <= 0f)
        {
            _attackCooldownTimer = attackCooldown;
            
            if (Mathf.Abs(input.mouseRelativePosition.X) > Mathf.Abs(input.mouseRelativePosition.Y))
            {
                if (input.mouseRelativePosition.X > 0)
                {
                    attackRight();
                }
                else
                {
                    attackLeft();
                }
            }
            else
            {
                if (input.mouseRelativePosition.Y < 0)
                {
                    attackUp();
                }
                else
                {
                    // attackDown();
                }
            }
        }
    }

    private void attackLeft()
    {
        _leftAttackHitbox.GetNode<CollisionShape2D>("Hitbox").Disabled = false;
        _leftHitboxTimer = attackDuration;
    }

    private void attackRight()
    {
        _rightAttackHitbox.GetNode<CollisionShape2D>("Hitbox").Disabled = false;
        _rightHitboxTimer = attackDuration;
    }

    private void attackUp()
    {
        _upAttackHitbox.GetNode<CollisionShape2D>("Hitbox").Disabled = false;
        _upHitboxTimer = attackDuration;
    }
}
