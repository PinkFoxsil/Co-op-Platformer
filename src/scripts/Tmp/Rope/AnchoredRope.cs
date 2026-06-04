using Godot;

public partial class AnchoredRope : Rope
{
    [ExportCategory("Anchors")]
    [Export] public CollisionObject2D _startObject;
    [Export] public CollisionObject2D _endObject;

    private PinJoint2D _startPinJoint;
    private PinJoint2D _endPinJoint;

    public override void _Ready()
    {
        tailPos = _startObject.GlobalPosition;
        headPos = _endObject.GlobalPosition;

        base._Ready();

        _startPinJoint = CreatePinJoint(Vector2.Zero, _startObject, GetStartSegment());
        _endPinJoint = CreatePinJoint(GetEndSegment().GetLengthOffset(1), GetEndSegment(), _endObject);
    }
}