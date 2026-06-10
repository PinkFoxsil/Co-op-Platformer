using Godot;

public partial class Rope
{
	private float _segmentLength;
	private float _segmentWidth;
	private float _segmentMass;

	private PhysicsMaterial _physicsMaterial;

	private uint _collisionLayer;
	private uint _collisionMask;

	private float _pinJointSoftness;
	private float _pinJointBias;

	private Vector2 _tailPosition;
	public Vector2 TailPosition {
		get => TailSegment != null ? TailSegment.TailPosition : _tailPosition;
	}

	private Vector2 _headPosition;
	public Vector2 HeadPosition {
		get => HeadSegment != null ? HeadSegment.HeadPosition : _headPosition;
	}

	public RopeSegment TailSegment => segments?[0];
	public RopeSegment HeadSegment => segments?[^1];

	// Setting Position/GlobalPosition won't work unless the rope is Frozen
	private bool _isFrozen = false;
	public bool Freeze
	{
		get => _isFrozen;
		set => SetFreeze(value);
	}

	private Node2D _parent;

	public RopeSegment[] segments;

	public Rope(
		Vector2 tailPosition,
		Vector2 headPosition,
		Node2D parent,
		float segmentLength,
		float segmentWidth,
		float segmentMass,
		PhysicsMaterial physicsMaterial,
		uint collisionLayer,
		uint collisionMask,
		float pinJointSoftness,
		float pinJointBias
		)
	{
		_parent = parent;

		_segmentLength = segmentLength;
		_segmentWidth = segmentWidth;
		_segmentMass = segmentMass;

		_physicsMaterial = physicsMaterial;

		_collisionLayer = collisionLayer;
		_collisionMask = collisionMask;

		_pinJointSoftness = pinJointSoftness;
		_pinJointBias = pinJointBias;

        segments = CreateSegments(tailPosition, headPosition);
		ConnectSegments(segments);
	}

	public void MoveHeadTo(Vector2 position)
	{
		if (HeadSegment == null)
		{
			GD.PushWarning("Rope was instructed to move, but there's nothing to move.");
			return;
		}

		Vector2 moveVector = position - HeadPosition;

		foreach (RopeSegment segment in segments)
		{
			segment.GlobalPosition += moveVector;
		}
	}

	public Rope JoinTo(Rope other)
	{
		if (segments.Length == 0)
		{
			GD.PushWarning("Rope was joined to an empty Rope.");
			return other;
		}

		if (other.segments.Length == 0)
		{
			GD.PushWarning("Rope was joined to an empty Rope.");
			return this;
		}

		int index = segments.Length;
		foreach (RopeSegment segment in other.segments)
		{
			segment.Name = GetRopeSegmentName(index);
			index++;
		}

		segments[^1].ConnectTo(other.segments[0], _pinJointSoftness, _pinJointBias);
		segments = [.. segments, .. other.segments];

		return this;
	}

	public void ExtendTailSegmentTo(Vector2 position)
	{
		if (TailSegment == null)
		{
			GD.PushWarning("Rope was instructed to extend its tail segment, but it doesn't have one.");
			return;
		}

		Vector2 tailHeadToPosVect = position - TailSegment.HeadPosition;
		float length = Mathf.Min(_segmentLength, tailHeadToPosVect.Length());

		NodePath originalNodeB = TailSegment.PinJoint.NodeB;
		TailSegment.PinJoint.NodeB = null;

		Vector2 newPos = TailSegment.HeadPosition - tailHeadToPosVect.Normalized() * length * 0.5f;
		TailSegment.Length = length;
		TailSegment.GlobalPosition = newPos;
		TailSegment.Rotation = tailHeadToPosVect.Angle();

		TailSegment.PinJoint.Position = TailSegment.GetLengthOffset(1f);
		TailSegment.PinJoint.NodeB = originalNodeB;
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
			segment.Freeze = value;
		}
	}

	private RopeSegment[] CreateSegments(Vector2 from, Vector2 to)
	{
		Vector2 startToEndVect = to - from;
		Vector2 direction = startToEndVect.Normalized();
		float rotation = direction.Angle();

        float length = startToEndVect.Length();

        float lastSegmentLength = length % _segmentLength;
        int amount = (int) (length / _segmentLength) + (lastSegmentLength > Mathf.Epsilon ? 1 : 0);

        RopeSegment[] ropeSegments = new RopeSegment[amount];

		int index = amount - 1;
        while (length > Mathf.Epsilon)
        {
            float currentSegmentLength = Mathf.Min(_segmentLength, length);
            Vector2 newSegmentPos = (length - currentSegmentLength * 0.5f) * direction + from;
            
            ropeSegments[index] = CreateSegment(index, currentSegmentLength, newSegmentPos, rotation);

            length -= currentSegmentLength;
            index--;
        }

		return ropeSegments;
	}

	private void ConnectSegments(RopeSegment[] segments)
	{
		for (int i = 1; i < segments.Length; i++)
		{
			RopeSegment currentSegment = segments[i - 1];
			RopeSegment nextSegment = segments[i];

			currentSegment.ConnectTo(nextSegment, _pinJointSoftness, _pinJointBias);
		}
	}

	private RopeSegment CreateSegment(int index, float length, Vector2 position, float rotation)
	{
		RopeSegment segment = new()
		{
			Name = GetRopeSegmentName(index),
			Length = length,
			Width = _segmentWidth,
			CollisionLayer = _collisionLayer,
			CollisionMask = _collisionMask,
			Mass = _segmentMass,
			PhysicsMaterialOverride = _physicsMaterial,
			Freeze = Freeze,
			Position = position,
			Rotation = rotation
		};

		_parent.AddChild(segment);

		return segment;
	}

	private static string GetRopeSegmentName(int index)
	{
		return $"RopeSegment_{index}";
	}
}
