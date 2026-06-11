using Godot;

public partial class AnchoredRopeController : RopeController
{
    [ExportCategory("Anchors")]
    [Export] public CollisionObject2D tailAnchor;
    [Export] public CollisionObject2D headAnchor;

    private PinJoint2D _tailPinJoint;

    public override void _Ready()
    {
        base._Ready();

        SetRope(tailAnchor.GlobalPosition, headAnchor.GlobalPosition);
        ConnectTailAnchorTo(rope.TailSegment);
        rope.HeadSegment.ConnectTo(headAnchor, softness, bias);
    }

    private void ConnectTailAnchorTo(CollisionObject2D other)
    {
        _tailPinJoint = new()
		{
			Name = "StartPinJoint",
			Position = Vector2.Zero,
			Softness = softness,
			Bias = bias,
			NodeA = tailAnchor.GetPath(),
			NodeB = other.GetPath()
		};

        tailAnchor.AddChild(_tailPinJoint);

        return;
    }
}