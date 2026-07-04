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
    [Export] public float nozzleDistance = 40f;
    [Export] public float trajectoryLength = 75f;
    [Export] public float harpoonFiringForce = 2000f;

    public HarpoonGunState State { get; private set; }

    private Harpoon _harpoon;
    private Rope _rope;
    private Line2D _trajectoryLine;

    public override void _Ready()
    {
        State = HarpoonGunState.Stashed;

        Show();

        _harpoon = GetNode<Harpoon>("Harpoon");
        _rope = GetNode<Rope>("Rope");
        _trajectoryLine = GetNode<Line2D>("TrajectoryLine");

        _trajectoryLine.Hide();
        _harpoon.Disable();
        _rope.Disable();

        _harpoon.OnMove += ExtendRope;
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

        if (_harpoon.ProcessMode == ProcessModeEnum.Disabled)
        {
            Debugger.Instance.StopDebugSimulation();
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
        UpdateTrajectoryLine();
    }

    private void Fire()
    {
        State = HarpoonGunState.Shot;
        _trajectoryLine.Hide();

        Debugger.Instance.StartDebugSimulation(); // TODO: remove or comment out

        _harpoon.Enable();
        _harpoon.GlobalPosition = GetStartPosition() - _harpoon.ropeAttachMarker.Position.Rotated(MouseUtility.GetMouseUnitVector(this).Angle())*2; // TODO: remove * 2
        _harpoon.Fire(MouseUtility.GetMouseUnitVector(this) * harpoonFiringForce);

        _rope.Enable();
        _rope.Init(GetStartPosition(), _harpoon.ropeAttachMarker.GlobalPosition);

        _rope.Freeze = true;
    }

    private Vector2 GetStartPosition()
    {
        return GlobalPosition + MouseUtility.GetMouseUnitVector(this) * nozzleDistance;
    }

    private void OnHarpoonLanded()
    {
        _rope.Freeze = false;
        Debugger.Instance.StopDebugSimulation();
    }

    private void ExtendRope(Transform2D newTransform)
    {
        if (_rope.segments.Length == 0)
        {
            _rope.Init(GetStartPosition(), newTransform * _harpoon.ropeAttachMarker.Position);
            return;
        }

        _rope.ExtendTo(_harpoon.ropeAttachMarker.GlobalPosition);
    }

    private void UpdateTrajectoryLine()
    {
        Vector2 p1 = ToLocal(GetStartPosition());
        Vector2 p2 = p1 + MouseUtility.GetMouseUnitVector(this) * trajectoryLength;

        _trajectoryLine.ClearPoints();
        _trajectoryLine.AddPoint(p1);
        _trajectoryLine.AddPoint(p2);
    }

    
}