using Godot;

public struct ShapeCastCollision
{
    public Vector2 Point { get; }
    public Vector2 Normal { get; }
    public GodotObject Object { get; }

    public ShapeCastCollision(ShapeCast2D shapeCast, int index)
    {
        Point = shapeCast.GetCollisionPoint(index);
        Normal = shapeCast.GetCollisionNormal(index);
        Object = shapeCast.GetCollider(index);
    }
}

public partial class Projectile : Area2D
{
    [Export] public float gravityScale = 0.1f;

    public Vector2 Velocity { get; set; }
    public bool Active { get; set; }

    public ShapeCast2D shapeCast;

    public override void _Ready()
    {
        CollisionShape2D collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        shapeCast = new()
        {
            CollisionMask = CollisionMask,
            Shape = collisionShape.Shape,
            TargetPosition = Vector2.Zero,
            Enabled = false
        };
        
        AddChild(shapeCast);

        Rotation = Velocity.Angle();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!Active)
        {
            return;
        }
        
        float dt = (float) delta;

        UpdateVelocity(dt);

        ShapeCastCollision? collision = GetClosestCollision(dt);
        if (collision != null)
        {
            OnHit(dt, (ShapeCastCollision) collision);
            return;
        }
        
        Move(dt);
    }

    public virtual void OnHit(float dt, ShapeCastCollision collision)
    {
        Position = Velocity * dt * shapeCast.GetClosestCollisionSafeFraction();
        Rotation = Velocity.Angle();
        Active = false;
    }

    private void UpdateVelocity(float dt)
    {
        Velocity += new Vector2(0, Gravity * gravityScale * dt);
    }

    private ShapeCastCollision? GetClosestCollision(float dt)
    {
        shapeCast.TargetPosition = ToLocal(GlobalPosition + Velocity * dt);
        shapeCast.ForceShapecastUpdate();

        return FindClosestCollision();
    }

    private ShapeCastCollision? FindClosestCollision()
    {
        float closestDistance = Mathf.Inf;
        ShapeCastCollision? closestCollision = null;

        int collisionCount = shapeCast.GetCollisionCount();

        for (int i = 0; i < collisionCount; i++)
        {
            ShapeCastCollision collision = new(shapeCast, i);

            float distance = collision.Point.DistanceTo(Position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCollision = collision;
            }
        }

        return closestCollision;
    }

    private void Move(float dt)
    {
        Rotation = Velocity.Angle();
        Position += Velocity * dt;
    }
}