using Godot;

public partial class Area2DProjectile : Area2D
{
    [Export] public float gravityScale = 0.1f;

    [Signal] public delegate void OnMoveEventHandler();
    [Signal] public delegate void OnHitEventHandler();

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

        shapeCast.ForceShapecastUpdate();

        Collision[] collisions = CollisionUtility.GetCollisions(shapeCast);
        Collision? closestCollision =  CollisionUtility.GetClosestCollision(collisions, Position);
        
        if (closestCollision != null)
        {
            Hit(dt, (Collision) closestCollision);
            return;
        }
        
        Move(dt);
    }

    public virtual void Hit(float dt, Collision collision)
    {
        EmitSignal(SignalName.OnHit);
    }

    private void UpdateVelocity(float dt)
    {
        Velocity += new Vector2(0, Gravity * gravityScale * dt);
        shapeCast.TargetPosition = ToLocal(GlobalPosition + Velocity * dt);
    }

    private void Move(float dt)
    {
        Rotation = Velocity.Angle();
        Position += Velocity * dt;

        EmitSignal(SignalName.OnMove);
    }
}