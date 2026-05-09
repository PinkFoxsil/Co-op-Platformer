using System.Runtime.CompilerServices;
using Godot;

public partial class HealthComponent : Component<Player>
{
    [Export] public int maxHealth = 100;

    private int _currentHealth;

    private Hitbox _hitbox;

    public override void Init(Entity<Player> entity)
    {
        base.Init(entity);

        _currentHealth = maxHealth;
        _hitbox = entity.node.GetNode<Node2D>("Hitboxes").GetNode<Hitbox>("PlayerHitbox");
    }

    public void AreaEntered(Area2D area)
    {
        GD.Print("Player hit by " + area.Name);
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            GD.Print("Player has died.");
        }
    }
}