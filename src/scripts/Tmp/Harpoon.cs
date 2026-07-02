using Godot;

public partial class Harpoon : RigidBody2D
{
    [Signal] public delegate void OnMoveEventHandler(Transform2D newTransform);

    public Marker2D ropeAttachMarker;

    public override void _Ready()
    {
        ropeAttachMarker = GetNode<Marker2D>("RopeAttachMarker");
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
        Show();
    }

    public void Disable()
    {
        ProcessMode = ProcessModeEnum.Disabled;
        Hide();
    }

    private void Land()
    {
        ProcessMode = ProcessModeEnum.Disabled;
    }
}