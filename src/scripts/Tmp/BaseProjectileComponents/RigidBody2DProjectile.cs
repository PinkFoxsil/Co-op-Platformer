using Godot;

public partial class RigidBody2DProjectile : Projectile<RigidBody2D>
{
    public override Vector2 LinearVelocity {
        get { return parent.LinearVelocity; }
        set { parent.LinearVelocity = value; }
    }

    public override void _Ready()
    {
        parent = GetParent<RigidBody2D>();
    }

    public override void _PhysicsProcess(double delta)
    {
        ResolveCollisions();
    }

    public override Collision[] GetCollisions()
    {
        PhysicsDirectBodyState2D state = PhysicsServer2D.BodyGetDirectState(parent.GetRid());
        return CollisionUtility.GetStateCollisions(state);
    }

    public override void Move()
    {
        base.Move();
    }

    public override void Hit(Collision collision)
    {
        base.Hit(collision);
    }
}