using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
	[Export] public float softness = 0.003f;
	[Export] public float bias = 0.99f;
	
	private float _halfSegmentLength;

	private Line2D _line2D;
	private List<Vector2> _line2DPoints = [];
	
	private LinkedList<RopeSegment> _ropeSegments = [];
	private LinkedList<PinJoint2D> _pinJoints = [];

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
		CreateRope();
	}

	public override void _Process(double dt)
	{
		UpdateLine2DRope();
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateStaticBodyPosition();
	}

	public void Resize()
	{
		MoveToEndMarker();
		FillStartSpace();
		//ConnectFirstRopeSegment();
	}

	

	private void MoveToEndMarker()
	{
		Vector2 difference = endMarker.GlobalPosition - _endStaticBody.GlobalPosition;
		_endStaticBody.Position += difference;
		_startStaticBody.Position += difference;

		foreach (RopeSegment segment in _ropeSegments)
		{
			segment.Position += difference;
		}
	}

	private void FillStartSpace()
	{
		DisconnectFirstRopeSegment();

		Vector2 difference = _ropeSegments.First.Value.GetPositionAlongLength(0) - _startStaticBody.GlobalPosition;
		
		float length = difference.Length();

		// Adjust the length of the first rope segment since it'll likely be short
		if (_ropeSegments.First != null && _ropeSegments.First.Value.Length != segmentLength)
		{
			RopeSegment firstRopeSegment = _ropeSegments.First.Value;
			PinJoint2D pinJoint = _pinJoints.First.Next.Value;
			
			Vector2 originalPosition = firstRopeSegment.GlobalPosition;
			Vector2 pinJointPosition = pinJoint.GlobalPosition;
			Vector2 pinToPosDirection = (originalPosition - pinJointPosition).Normalized();
			
			firstRopeSegment.RemoveChild(pinJoint);

			float correctiveLength = segmentLength - firstRopeSegment.Length;
			if (length > correctiveLength)
			{
				length -= correctiveLength;
				firstRopeSegment.Length = segmentLength;
				firstRopeSegment.GlobalPosition += pinToPosDirection * correctiveLength * 0.5f;
			}
			else
			{
				firstRopeSegment.Length += length;
				firstRopeSegment.GlobalPosition += pinToPosDirection * length * 0.5f;
			}

			pinJoint.Position = firstRopeSegment.GetPositionAlongLength(1);
			firstRopeSegment.AddChild(pinJoint);
		};

		GD.Print(length);

		// TODO: Fill the missing space with rope
		Vector2 endPos;
		if (_ropeSegments.First != null)
		{
			RopeSegment firstRopeSegment = _ropeSegments.First.Value;
			endPos = ToGlobal(firstRopeSegment.Position + firstRopeSegment.GetPositionAlongLength(0));
		}
		else
		{
			endPos = endMarker.GlobalPosition;
		}
		GD.Print(_startStaticBody.GlobalPosition, endPos);
		LinkedList<RopeSegment> ropeSegments = CreateSegments(_startStaticBody.GlobalPosition, endPos);
		LinkedList<PinJoint2D> pinJoints = ConnectRope(ropeSegments);

		GD.Print("Finish");

		CopyLinkedListToStart(_ropeSegments, ropeSegments);

		PinJoint2D oldFirstPin = _pinJoints.First.Value;
		_pinJoints.RemoveFirst();
		_pinJoints.AddFirst(CreatePinJoint(
			_ropeSegments.Last.Value.GetPositionAlongLength(1),
			_ropeSegments.Last.Value,
			ropeSegments.First.Value
			
		));
		CopyLinkedListToStart(_pinJoints, pinJoints);
		_pinJoints.AddFirst(oldFirstPin);
	}

	private void DisconnectFirstRopeSegment()
	{
		if (_pinJoints.First == null)
		{
			return;
		}

		_pinJoints.First.Value.NodeB = null;
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

	public void CreateRope()
	{
		Vector2 startPos = _startStaticBody.Position;
		Vector2 endPos = _endStaticBody.Position;

		_ropeSegments = CreateSegments(startPos, endPos);
		_pinJoints = ConnectRope(_ropeSegments);

		_pinJoints.AddFirst(ConnectStartToRopeSegment(_ropeSegments.First.Value));
		_pinJoints.AddLast(ConnectRopeSegmentToEnd(_ropeSegments.Last.Value));
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
			RopeSegment newSegment = CreateRopeSegment(segmentLength, newSegmentPos, rotation);
			ropeSegments.AddFirst(newSegment);

			ropeLength -= segmentLength;
		}

		// Add last segment to fill the remaining space
		if (ropeLength > Mathf.Epsilon)
		{
			Vector2 newSegmentPos = ropeLength * 0.5f * direction + startPoint;
			RopeSegment newSegment = CreateRopeSegment(ropeLength, newSegmentPos, rotation);
			ropeSegments.AddFirst(newSegment);
		}

		return ropeSegments;
	}

	private RopeSegment CreateRopeSegment(float length, Vector2 position, float rotation)
	{
		RopeSegment segment = new()
		{
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

	private LinkedList<PinJoint2D> ConnectRope(LinkedList<RopeSegment> ropeSegments)
	{
		LinkedList<PinJoint2D> pinJoints = [];

		LinkedListNode<RopeSegment> ropeNode = ropeSegments.First;
		while (ropeNode.Next != null)
		{
			pinJoints.AddLast(ConnectRopeSegments(
				ropeNode.Value,
				ropeNode.Next.Value
			));

			ropeNode = ropeNode.Next;
		}

		return pinJoints;
	}

	private PinJoint2D ConnectRopeSegments(RopeSegment a, RopeSegment b)
	{
		Vector2 pinJointPosition = a.GetPositionAlongLength(1);
		return CreatePinJoint(pinJointPosition, a, b);
	}

	private PinJoint2D ConnectStartToRopeSegment(RopeSegment ropeSegment)
	{
		return CreatePinJoint(Vector2.Zero, _startStaticBody, ropeSegment);
	}

	private PinJoint2D ConnectRopeSegmentToEnd(RopeSegment ropeSegment)
	{
		return CreatePinJoint(ropeSegment.GetPositionAlongLength(1), ropeSegment, _endStaticBody);
	}

	private PinJoint2D CreatePinJoint(Vector2 position, CollisionObject2D nodeA, CollisionObject2D nodeB)
	{
		PinJoint2D pinJoint = new()
		{
			Position = position,
			Softness = softness,
			Bias = bias,
			NodeA = nodeA.GetPath(),
			NodeB = nodeB.GetPath()
		};

		nodeA.AddChild(pinJoint);

		return pinJoint;
	}

	private void UpdateLine2DRope()
	{
		_line2DPoints.Clear();

		foreach (PinJoint2D pinJoint in _pinJoints)
		{
			_line2DPoints.Add(ToLocal(pinJoint.GlobalPosition));
		}

		_line2D.Points = _line2DPoints.ToArray();
	}
}
