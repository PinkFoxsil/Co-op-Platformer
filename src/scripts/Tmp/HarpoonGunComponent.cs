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

    private Line2D _trajectoryLine;
    private Marker2D _harpoonStartMaker;

    private PackedScene _harpoonPackedScene;
    private Harpoon _harpoon;

    private PackedScene _ropePackedScene;
    private Rope _rope;

    public override void _Ready()
    {
        State = HarpoonGunState.Stashed;

        _trajectoryLine = GetNode<Line2D>("TrajectoryLine");
        _trajectoryLine.Visible = false;

        _harpoonStartMaker = GetNode<Marker2D>("HarpoonStartMarker");

        _harpoonPackedScene = GD.Load<PackedScene>("res://src/scenes/harpoon.tscn");
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
        else if (State == HarpoonGunState.Aiming)
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
        else if (State == HarpoonGunState.Shot)
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

        if (_harpoon != null)
        {
            _harpoon.QueueFree();
            _harpoon = null;
        }
    }

    private void Aim()
    {
        State = HarpoonGunState.Aiming;
        _trajectoryLine.Visible = true;
    }

    private void UpdateAimTransform()
    {
        Rotation = GetMouseVector().Angle();
    }

    private void Fire()
    {
        State = HarpoonGunState.Shot;
        _trajectoryLine.Visible = false;

        _harpoon = _harpoonPackedScene.Instantiate<Harpoon>();
        _harpoon.GlobalPosition = _harpoonStartMaker.GlobalPosition;
        _harpoon.Velocity = GetMouseVector().Normalized() * 2000f;
        _harpoon.Active = true;
        AddChild(_harpoon);
    }

    private Vector2 GetMouseVector()
    {
        Vector2 mousePosition = GetGlobalMousePosition();
        return mousePosition - GlobalPosition;
    }

    private void Reel()
    {
        
    }
}