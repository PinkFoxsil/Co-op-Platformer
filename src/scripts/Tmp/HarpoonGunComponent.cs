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

public partial class HarpoonGunComponent : Node2D
{
    public HarpoonGunState State { get; private set; }

    private PackedScene _harpoonPackedScene;

    private Line2D _trajectoryLine;

    public override void _Ready()
    {
        _trajectoryLine = GetNode<Line2D>("TrajectoryLine");
        State = HarpoonGunState.Stashed;
        _trajectoryLine.Visible = false;
    }

    public override void _Process(double delta)
    {
        if (State == HarpoonGunState.Aiming)
        {
            UpdateAimTransform();
        }
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
        _trajectoryLine.Visible = false;
    }

    private void Aim()
    {
        State = HarpoonGunState.Aiming;
        _trajectoryLine.Visible = true;
    }

    private void UpdateAimTransform()
    {
        Vector2 mousePosition = GetGlobalMousePosition();
        Vector2 mouseVector = mousePosition - GlobalPosition;

        Rotation = mouseVector.Angle();
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