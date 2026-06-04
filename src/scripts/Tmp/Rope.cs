using Godot;
using System.Collections.Generic;

public struct RopeData
{
	public LinkedList<RopeSegment> segments;
	public LinkedList<PinJoint2D> pinJoints;
}

public partial class Rope : Node2D
{
	[ExportCategory("Rope")]
	[Export] public Marker2D startMarker;
	[Export] public Marker2D endMarker;

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
	private List<Vector2> _line2DPoints = [];
	
	private RopeData _ropeData;
	private PinJoint2D _startPinJoint;

	private StaticBody2D _startStaticBody;
	private StaticBody2D _endStaticBody;

	private PhysicsMaterial _ropeSegmentPhysicsMaterial;

	public override void _Ready()
	{
		_line2D = GetNode<Line2D>("Line2D");
		_startStaticBody = GetNode<StaticBody2D>("StartStaticBody");
		_endStaticBody = GetNode<StaticBody2D>("EndStaticBody");

		_halfSegmentLength = segmentLength * 0.5f;
		_ropeSegmentPhysicsMaterial = CreatePhysicsMaterial();

		UpdateStaticBodyPosition();
		_ropeData = CreateRope(startMarker.GlobalPosition, endMarker.GlobalPosition);

		_startPinJoint = ConnectStartToSegment(_ropeData.segments.First.Value);
		_ropeData.pinJoints.AddLast(ConnectSegmentToEnd(_ropeData.segments.Last.Value));
	}

	public override void _Process(double dt)
	{
		UpdateVisual();
		Debugger.Instance.DrawPoint(Vector2.Zero, Colors.Red);
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateStaticBodyPosition();
	}

	public void Resize()
	{
		DisconnectFirstPinJoint();
		MoveRopeToEnd();
		//FillStartSpace();
		//ConnectFirstRopeSegment();
	}

	private PhysicsMaterial CreatePhysicsMaterial()
	{
		return new()
		{
			Friction = friction
		};
	}

	private void UpdateStaticBodyPosition()
	{
		_startStaticBody.GlobalPosition = startMarker.GlobalPosition;
		_endStaticBody.GlobalPosition = endMarker.GlobalPosition;
	}

	public RopeData CreateRope(Vector2 startPos, Vector2 endPos)
	{
		LinkedList<RopeSegment> ropeSegments = CreateSegments(startPos, endPos);
		LinkedList<PinJoint2D> pinJoints = WeldSegments(ropeSegments);

		return new RopeData
		{
			segments = ropeSegments,
			pinJoints = pinJoints
		};
	}

	private LinkedList<RopeSegment> CreateSegments(Vector2 startPoint, Vector2 endPoint)
	{
		LinkedList<RopeSegment> ropeSegments = [];

		Vector2 startToEndVect = endPoint - startPoint;
		Vector2 direction = startToEndVect.Normalized();
		float rotation = direction.Angle();

		float ropeLength = startToEndVect.Length();
		while (ropeLength >= segmentLength)
		{
			Vector2 newSegmentPos = (ropeLength - _halfSegmentLength) * direction + startPoint;
			RopeSegment newSegment = CreateSegment($"RopeSegment_{ropeSegments.Count}", segmentLength, ToLocal(newSegmentPos), rotation);
			ropeSegments.AddFirst(newSegment);

			ropeLength -= segmentLength;
		}

		// Add last segment to fill the remaining space
		if (ropeLength > Mathf.Epsilon)
		{
			Vector2 newSegmentPos = ropeLength * 0.5f * direction + startPoint;
			RopeSegment newSegment = CreateSegment($"RopeSegment_{ropeSegments.Count}", ropeLength, ToLocal(newSegmentPos), rotation);
			ropeSegments.AddFirst(newSegment);
		}

		return ropeSegments;
	}

	private PinJoint2D ConnectStartToSegment(RopeSegment segment)
	{
		return CreatePinJoint($"PinJoint_Start", Vector2.Zero, _startStaticBody, segment);
	}

	private PinJoint2D ConnectSegmentToEnd(RopeSegment segment)
	{
		return CreatePinJoint($"PinJoint_End", segment.GetLengthOffset(1), segment, _endStaticBody);
	}

	private void MoveRopeToEnd()
	{
		if (_ropeData.segments.Last == null)
		{
			return;
		}

		RopeSegment lastSegment = _ropeData.segments.Last.Value;
		Vector2 difference = _endStaticBody.GlobalPosition - ToGlobal(lastSegment.ToLocal(lastSegment.GetLengthOffset(1)));

		foreach (RopeSegment segment in _ropeData.segments)
		{
			segment.TargetPosition = segment.GlobalPosition + difference;
		}
	}

	private void FillStartSpace()
	{
		if (_ropeData.segments.First == null)
		{
			_ropeData = CreateRope(startMarker.GlobalPosition, endMarker.GlobalPosition);
			return;
		}

		RopeSegment startSegment = _ropeData.segments.First.Value;

		// Get end position
		Vector2 endPos = GetRopeStartPosition(_ropeData.segments);
		Vector2 difference = endPos - startMarker.GlobalPosition;
		
		float length = difference.Length();

		// Adjust the length of the first rope segment since it'll likely be short
		if (startSegment.Length < segmentLength)
		{
			PinJoint2D pinJoint = _ropeData.pinJoints.First.Value;
			
			Vector2 originalPosition = startSegment.GlobalPosition;
			Vector2 offsetDirection = (endPos - originalPosition).Normalized();
			
			NodePath originalNodeA = pinJoint.NodeA;
			NodePath originalNodeB = pinJoint.NodeB;
			pinJoint.NodeA = null;
			pinJoint.NodeB = null;

			startSegment.RemoveChild(pinJoint);

			float correctiveLength = segmentLength - startSegment.Length;
			if (length > correctiveLength)
			{
				length -= correctiveLength;
				startSegment.Length = segmentLength;
				startSegment.Position += offsetDirection * correctiveLength * 0.5f;
			}
			else
			{
				startSegment.Length += length;
				startSegment.Position += offsetDirection * length * 0.5f;
			}

			pinJoint.Position = startSegment.GetLengthOffset(1);
			startSegment.AddChild(pinJoint);

			pinJoint.NodeA = originalNodeA;
			pinJoint.NodeB = originalNodeB;
		};

		// Fill the missing space with a new rope
		RopeData newRopeData = CreateRope(startMarker.GlobalPosition, endPos);

		// Connect the new rope to the existing rope
		// if (newRopeData.segments.Last != null)
		// {
		// 	_ropeData.pinJoints.AddFirst(ConnectSegments(newRopeData.segments.Last.Value, startSegment));
		// }

		CopyLinkedListToStart(_ropeData.segments, newRopeData.segments);
		CopyLinkedListToStart(_ropeData.pinJoints, newRopeData.pinJoints);
		
		// Reconnect the start pin joint
		// _startPinJoint.NodeB = _ropeData.segments.First.Value.GetPath();
	}

	private void DisconnectFirstPinJoint()
	{
		if (_startPinJoint == null)
		{
			return;
		}

		_startPinJoint.NodeB = null;
	}

	private Vector2 GetRopeStartPosition(LinkedList<RopeSegment> segments)
	{
		RopeSegment firstSegment = segments.First.Value;
		return firstSegment.GetLengthOffset(0).Rotated(firstSegment.Rotation) + firstSegment.GlobalPosition;
	}

	private RopeSegment CreateSegment(string Name, float length, Vector2 position, float rotation)
	{
		RopeSegment segment = new()
		{
			Name = Name,
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

	private LinkedList<PinJoint2D> WeldSegments(LinkedList<RopeSegment> segments)
	{
		LinkedList<PinJoint2D> pinJoints = [];

		LinkedListNode<RopeSegment> ropeNode = segments.First;
		while (ropeNode != null && ropeNode.Next != null)
		{
			pinJoints.AddLast(ConnectSegments(
				 $"PinJoint",
				ropeNode.Value,
				ropeNode.Next.Value
			));

			ropeNode = ropeNode.Next;
		}

		return pinJoints;
	}

	private PinJoint2D ConnectSegments(string name, RopeSegment a, RopeSegment b)
	{
		Vector2 pinJointPosition = a.GetLengthOffset(1);
		return CreatePinJoint(name, pinJointPosition, a, b);
	}

	private PinJoint2D CreatePinJoint(string Name,Vector2 position, CollisionObject2D parentNode, CollisionObject2D otherNode)
	{
		PinJoint2D pinJoint = new()
		{
			Name = Name,
			Position = position,
			Softness = softness,
			Bias = bias,
			NodeA = parentNode.GetPath(),
			NodeB = otherNode.GetPath()
		};

		parentNode.AddChild(pinJoint);

		return pinJoint;
	}

	private void CopyLinkedListToStart<T>(LinkedList<T> listA, LinkedList<T> listB)
	{
		LinkedListNode<T> currentNode = listB.Last;
		while (currentNode != null)
		{
			listA.AddFirst(currentNode.Value);
			currentNode = currentNode.Previous;
		}
	}

	private void UpdateVisual()
	{
		_line2DPoints.Clear();

		_line2DPoints.Add(ToLocal(_startPinJoint.GlobalPosition));
		foreach (PinJoint2D pinJoint in _ropeData.pinJoints)
		{
			_line2DPoints.Add(ToLocal(pinJoint.GlobalPosition));
		}

		_line2D.Points = _line2DPoints.ToArray();
	}
}
