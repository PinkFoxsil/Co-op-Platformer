using Godot;

public readonly struct Collision
{
    public Vector2 Position { get; init; }
    public Vector2 Normal { get; init; }
    public GodotObject Object { get; init; }
}

public static class CollisionUtility
{
    public static Collision? GetClosestCollision(Collision[] collisions, Vector2 position)
    {
        Collision? closestCollision = null;
        float closestDistance = Mathf.Inf;

        foreach (Collision collision in collisions)
        {
            if ((collision.Position - position).Length() <= closestDistance)
            {
                closestCollision = collision;
            }
        }

        return closestCollision;
    }

    public static Collision[] GetStateCollisions(PhysicsDirectBodyState2D state)
    {
        int contactCount = state.GetContactCount();

        Collision[] collisions = new Collision[contactCount];
        for (int i = 0; i < contactCount; i++)
        {
            collisions[i] = new Collision
            {
                Position = state.GetContactColliderPosition(i),
                Normal = state.GetContactLocalNormal(i),
                Object = state.GetContactColliderObject(i)
            };
        }

        return collisions;
    }

    public static Collision[] GetShapeCastCollisions(ShapeCast2D shapeCast)
    {
        int contactCount = shapeCast.GetCollisionCount();
        
        Collision[] collisions = new Collision[contactCount];
        for (int i = 0; i < contactCount; i++)
        {
            collisions[i] = new Collision
            {
                Position = shapeCast.GetCollisionPoint(i),
                Normal = shapeCast.GetCollisionNormal(i),
                Object = shapeCast.GetCollider(i)
            };
        }

        return collisions;
    }
}
