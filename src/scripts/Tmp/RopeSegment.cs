using Godot;

public partial class RopeSegment : RigidBody2D
{
    [Export] public float length = 20f;
    [Export] public float radius = 2f;

    public CollisionShape2D collisionShape;
    public CapsuleShape2D capsuleShape;

    private float _diameter;

    public override void _Ready()
    {
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        CapsuleShape2D capsuleShape = (CapsuleShape2D) collisionShape.Shape;

        _diameter = radius*2;
        capsuleShape.Height = length + _diameter;
        capsuleShape.Radius = radius;
    }
}