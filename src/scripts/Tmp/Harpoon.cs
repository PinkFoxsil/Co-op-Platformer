using Godot;

public partial class Harpoon : RigidBody2D
{
    [Signal] public delegate void OnMoveEventHandler(Transform2D newTransform);
    [Signal] public delegate void OnLandEventHandler();

    public Marker2D ropeAttachMarker;
    public bool landed = false;

    public override void _Ready()
    {
        ropeAttachMarker = GetNode<Marker2D>("RopeAttachMarker");
    }

    public override void _PhysicsProcess(double delta)
    {
        int contactCount = GetCollidingBodies().Count;

        if (contactCount > 0)
        {
            
        }
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        EmitSignal(SignalName.OnMove, state.Transform);

        Collision[] collisions = CollisionUtility.GetCollisions(state);
        Collision? closestCollision = CollisionUtility.GetClosestCollision(collisions, Position);

        if (closestCollision == null)
        {
            return;
        }

        Land();
    }

    public void Fire(Vector2 velocity)
    {
        LinearVelocity = velocity;
        GlobalRotation = velocity.Angle();
    }

    public void Enable()
    {
        ProcessMode = ProcessModeEnum.Inherit;
        Freeze = false;
        Show();
    }

    public void Disable()
    {
        ProcessMode = ProcessModeEnum.Disabled;
        Hide();
    }

    private void Land()
    {
        landed = true;
        SetDeferred(RigidBody2D.PropertyName.Freeze, true);
        EmitSignal(SignalName.OnLand);
    }
}