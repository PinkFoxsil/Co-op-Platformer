using Godot;

public partial class RopeController : Node2D
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

	public Rope rope;
	public Line2D line2D;

	public int SegmentAmount => rope.segments.Length;

	private PhysicsMaterial _physicsMaterial;

	public override void _Ready()
	{
		line2D = CreateLine2D();
		AddChild(line2D);

		_physicsMaterial = CreatePhysicsMaterial();
	}

	public override void _Process(double dt)
	{
		UpdateVisual();
	}

	public void ExtendTo(Vector2 position)
	{
		Vector2 oldTailPos = rope.TailPosition;

		rope.MoveHeadTo(position);
		rope.ExtendTailSegmentTo(position);

		Rope extendedRope = CreateRope(oldTailPos, rope.TailPosition);
		rope = extendedRope.JoinTo(rope);
	}

	public void SetRope(Vector2 tailPosition, Vector2 headPosition)
	{
		rope = CreateRope(tailPosition, headPosition);
	}

	public Rope CreateRope(Vector2 tailPosition, Vector2 headPosition)
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

	public void Clear()
	{
		foreach (RopeSegment segment in rope.segments)
		{
			segment.QueueFree();
		}

		rope = null;
	}

	public void Enable()
	{
		ProcessMode = ProcessModeEnum.Inherit;
		Show();
	}

	public void Disable()
	{
		ProcessMode = ProcessModeEnum.Disabled;
		Hide();
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
