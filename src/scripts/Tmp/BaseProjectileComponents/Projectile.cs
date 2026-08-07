using Godot;

public abstract partial class Projectile<T> : Node2D
{
    [Signal] public delegate void OnMoveEventHandler();
    [Signal] public delegate void OnHitEventHandler(GodotObject godotObject, Vector2 position, Vector2 normal);

    public virtual Vector2 LinearVelocity { get; set; }
    public T parent;

    public void ResolveCollisions()
    {
        Collision[] collisions = GetCollisions();
        Collision? closestCollision =  CollisionUtility.GetClosestCollision(collisions, Position);
        
        if (closestCollision == null)
        {
            Move();
            return;
        }
        
        Hit((Collision) closestCollision);
    }

    public abstract Collision[] GetCollisions();

    public virtual void Move()
    {
        EmitSignal(SignalName.OnMove);
    }

    public virtual void Hit(Collision collision)
    {
        EmitSignal(SignalName.OnHit, collision.Object, collision.Position, collision.Normal);
    }
}