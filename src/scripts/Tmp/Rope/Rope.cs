using Godot;

public partial class Rope : Node2D
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
	[Export] public float pinJointSoftness = 0.01f;
	[Export] public float pinJointBias = 0.99f;

	public Line2D line2D;

	public Vector2 TailPosition => TailSegment.TailPosition;
	public Vector2 HeadPosition => HeadSegment.HeadPosition;

	public RopeSegment TailSegment => segments.Length > 0 ? segments[0] : null;
	public RopeSegment HeadSegment => segments.Length > 0 ? segments[^1] : null;

	// Setting Position/GlobalPosition won't work unless the rope is Frozen
	private bool _isFrozen = false;
	public bool Freeze
	{
		get => _isFrozen;
		set => SetFreeze(value);
	}

	public RopeSegment[] segments = [];

	public float TotalLength {
		get {
			float length = 0f;
			foreach (RopeSegment segment in segments)
			{
				length += segment.Length;
			}
			return length;
		}
	}

	private PhysicsMaterial _physicsMaterial;

	public override void _Ready()
	{
		line2D = GetNode<Line2D>("Line2D");
		_physicsMaterial = CreatePhysicsMaterial();
	}

	public void Init(Vector2 from, Vector2 to)
	{
		segments = CreateConnectedSegments(from, to);
	}

	public void ExtendTailTo(Vector2 position)
	{
		ExtendTailSegmentTo(position);

		//RopeSegment[] extendedSegments = CreateConnectedSegments(position, TailPosition);
		//segments = JoinSegmentChains(extendedSegments, segments);
	}

	public void ClearSegments()
	{
		foreach (RopeSegment segment in segments)
		{
			segment.QueueFree();
		}

		segments = [];
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

	private RopeSegment[] CreateConnectedSegments(Vector2 from, Vector2 to)
	{
		RopeSegment[] rope = CreateSegments(from, to);
		ConnectSegments(rope);

		return rope;
	}

	private PhysicsMaterial CreatePhysicsMaterial()
	{
		return new()
		{
			Friction = friction
		};
	}

	private RopeSegment[] CreateSegments(Vector2 from, Vector2 to)
	{
		Vector2 startToEndVect = to - from;
		Vector2 direction = startToEndVect.Normalized();
		float rotation = direction.Angle();

        float length = startToEndVect.Length();

        float lastSegmentLength = length % segmentLength;
        int amount = (int) (length / segmentLength) + (lastSegmentLength > Mathf.Epsilon ? 1 : 0);

        RopeSegment[] ropeSegments = new RopeSegment[amount];

		int index = amount - 1;
        while (length > Mathf.Epsilon)
        {
            float currentSegmentLength = Mathf.Min(segmentLength, length);
            Vector2 newSegmentPos = (length - currentSegmentLength * 0.5f) * direction + from;
            
            ropeSegments[index] = CreateSegment(index, currentSegmentLength, newSegmentPos, rotation);

            length -= currentSegmentLength;
            index--;
        }

		return ropeSegments;
	}

	private RopeSegment CreateSegment(int index, float length, Vector2 position, float rotation)
	{
		RopeSegment segment = new()
		{
			Name = GetRopeSegmentName(index),
			Length = length,
			Width = width,
			CollisionLayer = layer,
			CollisionMask = mask,
			Mass = segmentMass,
			PhysicsMaterialOverride = _physicsMaterial,
			Freeze = Freeze,
			Position = position,
			Rotation = rotation
		};

		AddChild(segment);

		return segment;
	}

	private RopeSegment[] JoinSegmentChains(RopeSegment[] a, RopeSegment[] b)
	{
		int index = a.Length;
		foreach (RopeSegment segment in b)
		{
			segment.Name = GetRopeSegmentName(index);
			index++;
		}

		if (a.Length != 0 && b.Length != 0)
		{
			a[^1].ConnectTo(b[0], pinJointSoftness, pinJointBias);
		}
		
		return [.. a, .. b];
	}

	private void ExtendTailSegmentTo(Vector2 position)
	{
		if (TailSegment == null)
		{
			GD.PushWarning("Rope was instructed to extend its tail segment, but it doesn't have one.");
			return;
		}

		Vector2 posToOriginVect = TailSegment.HeadPosition - position;
		float length = Mathf.Min(segmentLength, posToOriginVect.Length());

		Vector2 newPos = TailSegment.HeadPosition - posToOriginVect.Normalized() * length * 0.5f;
		TailSegment.Length = length;
		TailSegment.SetPhysicsStateTransform(new(posToOriginVect.Angle(), newPos));

		if (TailSegment.PinJoint != null)
		{
			TailSegment.PinJoint.Position = TailSegment.ToLocal(segments.Length >= 1 ? segments[1].TailPosition : TailSegment.HeadPosition);
			TailSegment.UpdatePinJointAnchorOffset();
		}
	}

	private static string GetRopeSegmentName(int index)
	{
		return $"RopeSegment_{index}";
	}

	private void ConnectSegments(RopeSegment[] segments)
	{
		for (int i = 1; i < segments.Length; i++)
		{
			RopeSegment currentSegment = segments[i - 1];
			RopeSegment nextSegment = segments[i];

			currentSegment.ConnectTo(nextSegment, pinJointSoftness, pinJointBias);
		}
	}

	private void SetFreeze(bool value)
	{
		if (_isFrozen == value)
		{
			return;
		}

		_isFrozen = value;
		
		foreach (RopeSegment segment in segments)
		{
			segment.SetDeferred("Freeze", value);
		}
	}

	private void UpdateVisual()
	{
        Vector2[] points = new Vector2[segments.Length + 1];

		points[0] = TailPosition;
		for (int i = 1; i <= segments.Length; i++)
        {
            points[i] = ToLocal(segments[i - 1].HeadPosition);
        }

		line2D.Points = points;
	}
}
