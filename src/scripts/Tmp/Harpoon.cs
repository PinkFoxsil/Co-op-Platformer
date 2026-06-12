using Godot;

public partial class Harpoon : RigidBody2D
{
    public Marker2D ropeAttachMarker;

    public override void _Ready()
    {
        ropeAttachMarker = GetNode<Marker2D>("RopeAttachMarker");
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        Collision[] collisions = CollisionUtility.GetCollisions(state);
        Collision? closestCollision = CollisionUtility.GetClosestCollision(collisions, Position);

        if (closestCollision == null)
        {
            return;
        }

        
    }

    public void Fire(Vector2 velocity)
    {
        LinearVelocity = velocity;
        Rotation = velocity.Angle();
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
}