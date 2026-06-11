// Aim by holding Right Click
// Fire by pressing Left Click while aiming
// Reel by pressing Right Click while the gun is shot

using System;
using Godot;

public enum HarpoonGunState
{
    Stashed,
    Aiming,
    Shot
}

public partial class HarpoonGunComponent : Node2D
{
    [Export] public float nozzleDistance = 40f;
    [Export] public float harpoonFiringForce = 2000f;

    public HarpoonGunState State { get; private set; }

    private Harpoon _harpoon;
    private Rope _rope;
    private Line2D _trajectoryLine;

    public override void _Ready()
    {
        State = HarpoonGunState.Stashed;

        _harpoon = GetNode<Harpoon>("Harpoon");
        _rope = GetNode<Rope>("Rope");
        _trajectoryLine = GetNode<Line2D>("TrajectoryLine");

        _trajectoryLine.Hide();
        _harpoon.Disable();
        _rope.Disable();

        _harpoon.OnHit += () =>
        {
            ExtendRope();
            _harpoon.OnMove -= ExtendRope;
            _rope.Freeze = false;

            Debugger.Instance.StopDebugSimulation();
        };
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
                //Reel();
            }
        }
    }

    private void Stash()
    {
        State = HarpoonGunState.Stashed;

        _trajectoryLine.Hide();
        _harpoon.Disable();
        _rope.ClearSegments();
        _rope.Disable();
    }

    private void Aim()
    {
        State = HarpoonGunState.Aiming;
        _trajectoryLine.Show();
    }

    private void UpdateAimTransform()
    {
        Rotation = GetMouseVector().Angle();
    }

    private void Fire()
    {
        State = HarpoonGunState.Shot;
        _trajectoryLine.Hide();

        Debugger.Instance.StartDebugSimulation(); // TODO: remove or comment out

        _harpoon.Enable();
        FireHarpoon(GetMouseUnitVector() * harpoonFiringForce);

        _rope.Enable();
        _rope.Init(GetStartPosition(), _harpoon.ropeAttachMarker.GlobalPosition);

        _rope.Freeze = true;
        _harpoon.OnMove += ExtendRope;
    }

    private void FireHarpoon(Vector2 velocity)
    {
        _harpoon.GlobalPosition = GetStartPosition() - _harpoon.ropeAttachMarker.Position.Rotated(Rotation)*2; // TODO: remove * 2
        _harpoon.GlobalRotation = velocity.Angle();
        _harpoon.Velocity = velocity;
        _harpoon.Active = true;
    }

    private Vector2 GetStartPosition()
    {
        return GlobalPosition + GetMouseUnitVector() * nozzleDistance;
    }

    private void ExtendRope()
    {
        if (_rope.segments.Length == 0)
        {
            _rope.Init(GetStartPosition(), _harpoon.ropeAttachMarker.GlobalPosition);
            return;
        }

        _rope.ExtendTo(_harpoon.ropeAttachMarker.GlobalPosition);
    }

    // This can be moved to either a helper module or input component
    private Vector2 GetMouseUnitVector()
    {
        return GetMouseVector().Normalized();
    }

    // This can be moved to either a helper module or input component
    private Vector2 GetMouseVector()
    {
        Vector2 mousePosition = GetGlobalMousePosition();
        return mousePosition - GlobalPosition;
    }
}