using Godot;

public partial class Harpoon : RigidBody2D
{
    [Signal] public delegate void OnMoveEventHandler();
    [Signal] public delegate void OnLandEventHandler();

    public Marker2D ropeAttachMarker;

    private Transform2D _lastTransform;

    public override void _Ready()
    {
        ropeAttachMarker = GetNode<Marker2D>("RopeAttachMarker");
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        Collision[] collisions = CollisionUtility.GetStateCollisions(state);
        Collision? closestCollision = CollisionUtility.GetClosestCollision(collisions, Position);

        if (closestCollision == null)
        {
            OnProjectileMove();
            return;
        }
        
        OnProjectileLand();
        return;
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

    private void OnProjectileMove()
    {
        _lastTransform = Transform;
        EmitSignal(SignalName.OnMove);
    }

    private void OnProjectileLand()
    {
        Transform = _lastTransform;
        SetDeferred(RigidBody2D.PropertyName.Freeze, true);
        EmitSignal(SignalName.OnLand);
    }
}