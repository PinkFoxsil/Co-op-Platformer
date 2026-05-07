using Godot;
using System;

public partial class AttackComponent : Component
{
    [Export] public float attackCooldown = 1f;
    [Export] public float attackDuration = 0.5f;
    [Export] public int attackDamage = 10;

    private float _attackCooldownTimer;
    private float _attackDurationTimer;

    public override void PhysicsProcess(float dt)
    {
        InputComponent input = entity.GetComponent<InputComponent>();
        if (input == null)
        {
            return;
        }
    }
}
