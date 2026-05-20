using Godot;
using Godot.Collections;

public struct ShapeCastCollision
{
    public Vector2 Point;
    public Vector2 Normal;
    public GodotObject Object;
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
            Shape = collisionShape.Shape,
            Position = Position,
            TargetPosition = Position + Velocity,
            Enabled = false
        };
    }

    public override void _PhysicsProcess(double dt)
    {
        if (!Active)
        {
            return;
        }
        
        UpdateVelocity((float) dt);

        ShapeCastCollision? collision = GetClosestCollision();
        if (collision != null)
        {
            OnHit((ShapeCastCollision) collision);
            return;
        }
        
        Move();
    }

    public virtual void OnHit(ShapeCastCollision collision)
    {
        Position = Velocity * shapeCast.GetClosestCollisionSafeFraction();
        Rotation = Velocity.Angle() - Mathf.Pi / 2;
        Active = false;
    }

    private void UpdateVelocity(float dt)
    {
        Velocity += new Vector2(0, Gravity * gravityScale * dt);
    }

    private ShapeCastCollision? GetClosestCollision()
    {
        shapeCast.TargetPosition = Position + Velocity;
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
            ShapeCastCollision collision = new()
            {
                Point = shapeCast.GetCollisionPoint(i),
                Normal = shapeCast.GetCollisionNormal(i),
                Object = shapeCast.GetCollider(i)
            };

            float distance = collision.Point.DistanceTo(Position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCollision = collision;
            }
        }

        return closestCollision;
    }

    private void Move()
    {
        Rotation = Velocity.Angle() - Mathf.Pi / 2;
        Position += Velocity;
    }
}