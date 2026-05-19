// Aim by holding Right Click
// Fire by pressing Left Click while aiming
// Reel by pressing Right Click while the gun is shot

using Godot;

public enum HarpoonGunState
{
    Stashed,
    Aiming,
    Shot
}

public partial class HarpoonGun : RigidBody2D
{
    public HarpoonGunState State { get; private set; }

    private RigidBody2D _harpoon;
    private Rope _rope;

    public override void _Ready()
    {
        _harpoon = GetNode<RigidBody2D>("Harpoon");
        _rope = GetNode<Rope>("rope");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (State == HarpoonGunState.Stashed)
        {
            if (Input.IsActionPressed("Attack2"))
            {
                Aim();
            }
        }

        if (State == HarpoonGunState.Aiming)
        {
            if (Input.IsActionJustPressed("Attack1"))
            {
                Fire();
            }
            else if (!Input.IsActionPressed("Attack2"))
            {
                Stash();
            }
        }

        if (State == HarpoonGunState.Shot)
        {
            if (Input.IsActionJustPressed("Attack1"))
            {
                Stash();
            }
            else if (Input.IsActionJustPressed("Attack2"))
            {
                Reel();
            }
        }
    }

    private void Stash()
    {
        State = HarpoonGunState.Stashed;

        // Transform = stashedTransform;
        // VisibilityLayer = -1;
    }

    private void Aim()
    {
        State = HarpoonGunState.Aiming;
        UpdateAimTransform();
    }

    private void UpdateAimTransform()
    {
        // Transform = pivotTransform - GetGlobalMousePosition;
    }

    private void Fire()
    {
        State = HarpoonGunState.Shot;

        // _harpoon.launch(direction, speed);
    }

    private void Reel()
    {
        // _rope.length -= 1;
    }
}