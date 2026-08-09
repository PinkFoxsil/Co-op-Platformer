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
    private Transform2D _harpoonTransform;
    private PinJoint2D _ropeAttachJoint;
    private Rope _rope;
    private Line2D _trajectoryLine;

    private Vector2 _fireMouseDirection;

    public override void _Ready()
    {
        State = HarpoonGunState.Stashed;

        Show();

        _harpoon = GetNode<Harpoon>("Harpoon");
        _ropeAttachJoint = _harpoon.GetNode<PinJoint2D>("RopeAttachment");
        _rope = GetNode<Rope>("Rope");
        _trajectoryLine = GetNode<Line2D>("TrajectoryLine");

        _trajectoryLine.Hide();
        _harpoon.Disable();
        _rope.Disable();

        _harpoon.BodyEntered += OnHarpoonHit;
    }

    public override void _Process(double delta)
    {
        if (State == HarpoonGunState.Aiming)
        {
            UpdateTrajectoryLine();
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
            else if (Input.IsActionPressed("Attack2"))
            {
                //Reel();
            }
            else if (!_harpoon.Freeze)
            {
                OnHarpoonMove();
            }
        }
    }

    private void Stash()
    {
        State = HarpoonGunState.Stashed;
        _ropeAttachJoint.NodeA = null;

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

    private void Fire()
    {
        State = HarpoonGunState.Shot;
        _trajectoryLine.Hide();

        Debugger.Instance.StartDebugSimulation(); // TODO: remove or comment out

        FireHarpoon();

        _rope.Enable();
    }

    private Vector2 GetStartPosition()
    {
        return GlobalPosition + _fireMouseDirection * nozzleDistance;
    }

    private void FireHarpoon()
    {
        _harpoon.Enable();
        
        Vector2 harpoonVelocity = MouseUtility.GetMouseUnitVector(this) * harpoonFiringForce;
        Vector2 position = GetStartPosition() - _ropeAttachJoint.Position.Rotated(MouseUtility.GetMouseUnitVector(this).Angle());
        _harpoon.SetPhysicsStateTransform(new Transform2D(harpoonVelocity.Angle(), position));
        _harpoon.SetPhysicsStateLinearVelocity(harpoonVelocity);
    }

    private void OnHarpoonHit(Node body)
    {
        //_rope.Freeze = false;
        Debugger.Instance.StopDebugSimulation();
    }

    private void OnHarpoonMove()
    {
        ExtendRope();
    }

    private void ExtendRope()
    {
        if (_rope.segments.Length == 0)
        {
            CreateRope();
            return;
        }

        _rope.ExtendTailTo(GetStartPosition());
    }

    private void CreateRope()
    {
        _rope.Init(GetStartPosition(), _ropeAttachJoint.GlobalPosition);
        if (_rope.HeadSegment != null)
        {
            _ropeAttachJoint.NodeA = _rope.HeadSegment.GetPath();
        }
    }

    private void UpdateTrajectoryLine()
    {
        _fireMouseDirection = MouseUtility.GetMouseUnitVector(this);
        Vector2 p1 = ToLocal(GetStartPosition());
        Vector2 p2 = p1 + MouseUtility.GetMouseUnitVector(this) * trajectoryLength;

        _trajectoryLine.ClearPoints();
        _trajectoryLine.AddPoint(p1);
        _trajectoryLine.AddPoint(p2);
    }
}