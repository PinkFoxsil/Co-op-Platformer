using System.Runtime.CompilerServices;
using Godot;

public partial class HealthComponent
{
    private int _currentHealth;
    private Hitbox _hitbox;

    public HealthComponent(int maxHealth, Hitbox hitbox)
    {
        _currentHealth = maxHealth;
        _hitbox = hitbox;
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
        }
    }
}