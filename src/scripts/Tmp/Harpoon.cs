using Godot;
using Godot.Collections;

public partial class Harpoon : RigidBody2D
{
    private bool isActive;

    public override void _PhysicsProcess(double dt)
    {
        if (isActive)
        {
            DetectCollision();
        }
    }

    public void Launch()
    {
        isActive = true;
    }

    private void DetectCollision()
    {
        Array<Node2D> bodies = GetCollidingBodies();
        if (bodies[0] == null)
        {
            return;
        }

        AttachTo(bodies[0], Vector2.Zero);
    }

    private void AttachTo(Node2D entity, Vector2 offset)
    {
        
    }
}