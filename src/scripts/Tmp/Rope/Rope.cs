using Godot;

public partial class Rope : Node2D
{
	[ExportCategory("Attachment Positions")]
	[Export] public Vector2 startPos;
	[Export] public Vector2 endPos;

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
	
	private float _halfSegmentLength;

	private Line2D _line2D;
	
	private RopeSegment[] _segments;
    private PinJoint2D[] _pinJoints;

	private PinJoint2D _startPinJoint;

	private PhysicsMaterial _ropeSegmentPhysicsMaterial;

	public override void _Ready()
	{
		Name = "Rope";

		_line2D = new Line2D()
		{
			Name = "Line2D",
			Width = width
		};
		AddChild(_line2D);

		_halfSegmentLength = segmentLength * 0.5f;
		_ropeSegmentPhysicsMaterial = CreatePhysicsMaterial();

        _segments = CreateSegments(startPos, endPos);
		_pinJoints = JoinSegments(_segments);
	}

	public override void _Process(double dt)
	{
		UpdateVisual();
	}

	public Vector2 GetStartPosition()
	{
		RopeSegment firstSegment = _segments[0];
		return firstSegment.TailPosition;
	}

    public Vector2 GetEndPosition()
	{
		RopeSegment firstSegment = _segments[_segments.Length - 1];
		return firstSegment.HeadPosition;
	}
	
	public RopeSegment GetStartSegment()
	{
		return _segments[0];
	}

	public RopeSegment GetEndSegment()
	{
		return _segments[_segments.Length - 1];
	}

	private PhysicsMaterial CreatePhysicsMaterial()
	{
		return new()
		{
			Friction = friction
		};
	}

	private RopeSegment[] CreateSegments(Vector2 startPoint, Vector2 endPoint)
	{
		Vector2 startToEndVect = endPoint - startPoint;
		Vector2 direction = startToEndVect.Normalized();
		float rotation = direction.Angle();

        float length = startToEndVect.Length();

        float lastSegmentLength = length % segmentLength;
        int amount = (int) (length / segmentLength) + (lastSegmentLength > Mathf.Epsilon ? 1 : 0);

        RopeSegment[] ropeSegments = new RopeSegment[amount];

        while (length > Mathf.Epsilon)
        {
            int index = amount - 1;
            float currentSegmentLength = Mathf.Min(segmentLength, length);
            Vector2 newSegmentPos = (length - currentSegmentLength * 0.5f) * direction + startPoint;
            
            ropeSegments[index] = CreateSegment(index, currentSegmentLength, ToLocal(newSegmentPos), rotation);

            length -= currentSegmentLength;
            amount--;
        }

		return ropeSegments;
	}

	private RopeSegment CreateSegment(int index, float length, Vector2 position, float rotation)
	{
		RopeSegment segment = new()
		{
			Name = $"RopeSegment_{index}",
			Length = length,
			Width = width,
			CollisionLayer = layer,
			CollisionMask = mask,
			Mass = segmentMass,
			PhysicsMaterialOverride = _ropeSegmentPhysicsMaterial,
			Position = position,
			Rotation = rotation
		};
		
		AddChild(segment);

		return segment;
	}

	private PinJoint2D[] JoinSegments(RopeSegment[] segments)
	{
		PinJoint2D[] pinJoints = new PinJoint2D[segments.Length - 1];

        for (int i = 0; i < segments.Length - 1; i++)
        {
            RopeSegment currentSegment = segments[i];
            RopeSegment nextSegment = segments[i + 1];

            pinJoints[i] = ConnectSegments(
				currentSegment,
				nextSegment
			);
        }

        return pinJoints;
	}

	private PinJoint2D ConnectSegments(RopeSegment a, RopeSegment b)
	{
		Vector2 pinJointPosition = a.GetLengthOffset(1);
		return CreatePinJoint(pinJointPosition, a, b);
	}

	public PinJoint2D CreatePinJoint(Vector2 position, CollisionObject2D parentNode, CollisionObject2D otherNode)
	{
		PinJoint2D pinJoint = new()
		{
			Name = "PinJoint",
			Position = position,
			Softness = softness,
			Bias = bias,
			NodeA = parentNode.GetPath(),
			NodeB = otherNode.GetPath()
		};

		parentNode.AddChild(pinJoint);

		return pinJoint;
	}

	private void UpdateVisual()
	{
        Vector2[] points = new Vector2[_pinJoints.Length + 2];

		points[0] = GetStartPosition();

		for (int i = 0; i < _pinJoints.Length; i++)
        {
            points[i + 1] = ToLocal(_pinJoints[i].GlobalPosition);
        }
		
        points[^1] = GetEndPosition();

		_line2D.Points = points;
	}
}
