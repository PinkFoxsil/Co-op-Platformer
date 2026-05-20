using Godot;
using System;
using System.Collections.Generic;

public partial class Rope : Node2D
{
	[Export] public Marker2D startMarker;
	[Export] public Marker2D endMarker;
	[Export] public float segmentLength = 20f;
	[Export] public float softness = 0.003f;
	
	private float _halfSegmentLength;

	private Line2D _line2D;
	private List<Vector2> _line2DPoints = [];
	
	private PackedScene _ropeSegmentPackedScene;
	private LinkedList<RopeSegment> _ropeSegments = [];
	private LinkedList<PinJoint2D> _pinJoints = [];

	private StaticBody2D _startStaticBody;
	private StaticBody2D _endStaticBody;

    public override void _Ready()
	{
		_ropeSegmentPackedScene = GD.Load<PackedScene>("res://src/scenes/rope_segment.tscn");
		
		_line2D = GetNode<Line2D>("Line2D");

		_halfSegmentLength = segmentLength * 0.5f;

		CreateRope();
	}

    public override void _Process(double dt)
    {
        UpdateLine2DRope();
    }

    public override void _PhysicsProcess(double delta)
    {
        _startStaticBody.GlobalPosition = startMarker.GlobalPosition;
		_endStaticBody.GlobalPosition = endMarker.GlobalPosition;
    }

	public void CreateRope()
	{
		Vector2 startPos = _startStaticBody.Position;
		Vector2 endPos = _endStaticBody.Position;

		CreateSegments(startPos, endPos);
	}

	private void CreateSegments(Vector2 startPoint, Vector2 endPoint)
	{
		Vector2 startToEndVect = startPoint - endPoint;
		float distance = startToEndVect.Length();
		Vector2 direction = startToEndVect.Normalized();

		float rotation = direction.Angle() - Mathf.Pi / 2;

		float targetDistance = distance - _halfSegmentLength;
		for (float interval = _halfSegmentLength; interval <= targetDistance; interval += segmentLength)
		{
			Vector2 newSegmentPos = interval * direction + startPoint;
			RopeSegment newSegment = CreateRopeSegment(newSegmentPos, rotation);
			_ropeSegments.AddLast(newSegment);
		}
	}

	private RopeSegment CreateRopeSegment(Vector2 position, float rotation)
	{
		RopeSegment segment = _ropeSegmentPackedScene.Instantiate<RopeSegment>();
		segment.Position = position;
		segment.Rotation = rotation;
		segment.length = segmentLength;
		AddChild(segment);

		return segment;
	}

	private void ConnectRopeSegments()
	{
		if (_ropeSegments.First == null)
		{
			_pinJoints.AddFirst(
				CreatePinJoint(_startStaticBody.Position, _startStaticBody, _endStaticBody)
			);
			return;
		}

		ConnectStartRopeSegment();
		ConnectBetweenRopeSegments();
		ConnectEndRopeSegment();
	}

	private void ConnectStartRopeSegment()
	{
		PinJoint2D pinJoint = CreatePinJoint(_startStaticBody.Position, _startStaticBody, _ropeSegments.First.Value);
		_pinJoints.AddFirst(pinJoint);
	}

	private void ConnectBetweenRopeSegments()
	{
		LinkedListNode<RopeSegment> ropeNode = _ropeSegments.First;

		while (ropeNode.Next != null)
		{
			RopeSegment ropeA = ropeNode.Value;
			RopeSegment ropeB = ropeNode.Next.Value;

			Vector2 pinJointPosition = new(0, ropeA.length * 0.5f);
			_pinJoints.AddLast(CreatePinJoint(pinJointPosition, ropeA, ropeB));

			ropeNode = ropeNode.Next;
		}
	}

	private void ConnectEndRopeSegment()
	{
		PinJoint2D pinJoint = CreatePinJoint(_ropeSegments.Last.Value.Position, _ropeSegments.Last.Value, _endStaticBody);
		_pinJoints.AddLast(pinJoint);
	}

	private static PinJoint2D CreatePinJoint(Vector2 position, CollisionObject2D nodeA, CollisionObject2D nodeB)
	{
        PinJoint2D pinJoint = new()
        {
            Position = position,
            NodeA = nodeA.GetPath(),
            NodeB = nodeB.GetPath()
        };

        pinJoint.AddChild(nodeA);

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
