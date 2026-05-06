using Godot;
using System;

public class DashComponent : IComponent
{
    public float DashSpeed = 100f;
    public float DashTime = 0.1f;
    public float DashCooldown = 0.3f;
    public int DashCount;

    public float DashTimer;
    public float CooldownTimer;
    public bool isDashing;
}
