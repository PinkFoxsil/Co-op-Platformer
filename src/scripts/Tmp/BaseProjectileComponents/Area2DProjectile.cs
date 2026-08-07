using Godot;

public partial class Area2DProjectile : Projectile<Area2D>
{
    [Export] public float gravityScale = 0.1f;

    private ShapeCast2D _shapeCast;

    public override void _Ready()
    {
        parent = GetParent<Area2D>();

        CollisionShape2D collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        _shapeCast = new()
        {
            CollisionMask = parent.CollisionMask,
            Shape = collisionShape.Shape,
            TargetPosition = Vector2.Zero,
            Enabled = false
        };
        
        AddChild(_shapeCast);

        Rotation = LinearVelocity.Angle();
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float) delta;

        UpdateVelocity(dt);

        _shapeCast.ForceShapecastUpdate();

        ResolveCollisions();
    }

    public override Collision[] GetCollisions()
    {
        return CollisionUtility.GetShapeCastCollisions(_shapeCast);
    }

    private void UpdateVelocity(float dt)
    {
        _shapeCast.TargetPosition = ToLocal(GlobalPosition + LinearVelocity * dt);
    }

    private void Move(float dt)
    {
        parent.Rotation = LinearVelocity.Angle();
        parent.Position += LinearVelocity * dt;

        base.Move();
    }

    public override void Hit(Collision collision)
    {
        base.Hit(collision);
    }
}