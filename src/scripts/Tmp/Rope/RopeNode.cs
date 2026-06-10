using Godot;

public partial class RopeNode : Node2D
{
	[ExportCategory("Rope Segment")]
	[Export] public float segmentLength = 20f;
	[Export] public float segmentMass = 25f;
	[Export] public float friction = 0.1f;
	[Export] public float width = 2f;

	[ExportGroup("Collision")]
	[Export(PropertyHint.Layers2DPhysics)] public uint layer;
	[Export(PropertyHint.Layers2DPhysics)] public uint mask;

	[ExportCategory("Pin Joint")]
	[Export] public float softness = 0.01f;
	[Export] public float bias = 0.99f;

	public Vector2 TailPosition;
	public Vector2 HeadPosition;

	public Rope rope;
	public Line2D line2D;

	private PhysicsMaterial _physicsMaterial;

	public override void _Ready()
	{
		Name = "Rope";

		line2D = CreateLine2D();
		AddChild(line2D);

		_physicsMaterial = CreatePhysicsMaterial();

        rope = CreateRope(TailPosition, HeadPosition);
	}

	public override void _Process(double dt)
	{
		UpdateVisual();
	}

	public void ExtendTo(Vector2 position)
	{
		Vector2 oldTailPos = TailPosition;

		rope.MoveHeadTo(position);
		rope.ExtendTailSegmentTo(position);

		Rope newSegments = CreateRope(oldTailPos, TailPosition);

		//newSegments.JoinTo(rope);
	}

	private Line2D CreateLine2D()
	{
		return new()
		{
			Name = "Line2D",
			Width = width
		};
	}

	private PhysicsMaterial CreatePhysicsMaterial()
	{
		return new()
		{
			Friction = friction
		};
	}

	private Rope CreateRope(Vector2 tailPosition, Vector2 headPosition)
	{
        return new(
            tailPosition,
            headPosition,
			this,
            segmentLength,
            width,
            segmentMass,
            _physicsMaterial,
            layer,
            mask,
            softness,
            bias
        );
	}

	private void UpdateVisual()
	{
		RopeSegment[] segments = rope.segments;
        Vector2[] points = new Vector2[segments.Length + 1];

		points[0] = rope.TailPosition;
		for (int i = 1; i <= segments.Length; i++)
        {
            points[i] = ToLocal(segments[i - 1].HeadPosition);
        }

		line2D.Points = points;
	}
}
