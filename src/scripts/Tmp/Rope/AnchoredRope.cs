using Godot;

public partial class AnchoredRope : Rope
{
    [ExportCategory("Anchors")]
    [Export] public CollisionObject2D tailAnchor;
    [Export] public CollisionObject2D headAnchor;

    private PinJoint2D _tailPinJoint;
    private PinJoint2D _headPinJoint;

    public override void _Ready()
    {
        TailPosition = tailAnchor.GlobalPosition;
        HeadPosition = headAnchor.GlobalPosition;

        base._Ready();

        _tailPinJoint = CreatePinJoint(Vector2.Zero, tailAnchor, TailSegment);
        _headPinJoint = CreatePinJoint(HeadSegment.GetLengthOffset(1f), HeadSegment, headAnchor);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}