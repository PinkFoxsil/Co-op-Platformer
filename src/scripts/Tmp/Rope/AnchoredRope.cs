using Godot;

public partial class AnchoredRope : Rope
{
    [ExportCategory("Anchors")]
    [Export] public CollisionObject2D tailAnchor;
    [Export] public CollisionObject2D headAnchor;

    private PinJoint2D _tailPinJoint;

    public override void _Ready()
    {
        base._Ready();

        Init(tailAnchor.GlobalPosition, headAnchor.GlobalPosition);
        ConnectTailAnchorTo(TailSegment);
        HeadSegment.ConnectTo(headAnchor, pinJointSoftness, pinJointBias);
    }

    private void ConnectTailAnchorTo(CollisionObject2D other)
    {
        _tailPinJoint = new()
		{
			Name = "StartPinJoint",
			Position = Vector2.Zero,
			Softness = pinJointSoftness,
			Bias = pinJointBias,
			NodeA = tailAnchor.GetPath(),
			NodeB = other.GetPath()
		};

        tailAnchor.AddChild(_tailPinJoint);

        return;
    }
}