using Godot;
using System;

public partial class CharacterColliderComponent : Node, IComponent
{
    [Export] public float mass = 40f;
    [Export] public float bounce = 0.0f;

    private float _halfMass;
    private float _inverseMass;

	private Player _character;
	
	public void Init(Node parentNode)
	{
        _character = (Player) parentNode;
	}

    public void PhysicsProcess(float dt)
    {
        KinematicCollision2D[] nodeCollisions = GetSlideCollisions();
        KinematicCollision2D[] rigidBodyCollisions = FilterCollisionRigidBodies(nodeCollisions);

        if (rigidBodyCollisions.Length == 0)
        {
            return;
        }

        foreach (KinematicCollision2D rigidBodyCollision in rigidBodyCollisions)
        {
            Vector2 normal = rigidBodyCollision.GetNormal();
            Vector2 position = rigidBodyCollision.GetPosition();

            GodotObject nodeCollided = rigidBodyCollision.GetCollider();
            RigidBody2D rigidBody = (RigidBody2D) nodeCollided;
            
            rigidBody.ApplyCentralImpulse(-normal * mass);
        }
    }

    private KinematicCollision2D[] GetSlideCollisions()
    {
        int collisionCount = _character.GetSlideCollisionCount();
        KinematicCollision2D[] kinematicCollisions = new KinematicCollision2D[collisionCount];

        for (int i = 0; i < collisionCount; i++)
        {
            kinematicCollisions[i] = _character.GetSlideCollision(i);
        }

        return kinematicCollisions;
    }

    private KinematicCollision2D[] FilterCollisionRigidBodies(KinematicCollision2D[] collisions)
    {
        KinematicCollision2D[] rigidBodyCollisions = new KinematicCollision2D[collisions.Length];

        int arraySize = 0;
        foreach (KinematicCollision2D collision in collisions)
        {
            GodotObject nodeCollided = collision.GetCollider();
            if (nodeCollided is RigidBody2D)
            {
                rigidBodyCollisions[arraySize] = collision;
                arraySize++;
            }
        }

        Array.Resize(ref rigidBodyCollisions, arraySize);
        return rigidBodyCollisions;
    }
}