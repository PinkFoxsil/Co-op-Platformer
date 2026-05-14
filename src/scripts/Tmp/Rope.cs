using Godot;
using System;
using System.Collections.Generic;

public partial class Rope : Node2D
{
	[Export] public bool staticRopeEnd = false;
	[Export] public float segmentLength = 10f;

	private Line2D _line2D;
	private List<Vector2> _line2DPoints = [];

	private RopeSegment _ropeStart;
	private RopeSegment _ropeEnd;
	private PinJoint2D _ropeStartPinJoint;
	private PinJoint2D _ropeEndPinJoint;
	
	private PackedScene _ropeSegmentPackedScene;
	private List<RopeSegment> _ropeSegments = [];

    public override void _Ready()
	{
		_ropeSegmentPackedScene = GD.Load<PackedScene>("res://src/scenes/rope_segment.tscn");
		
		_line2D = GetNode<Line2D>("Line2D");
		_ropeStart = GetNode<RopeSegment>("RopeStart");
		_ropeEnd = GetNode<RopeSegment>("RopeEnd");
		_ropeStartPinJoint = GetNode<PinJoint2D>("RopeStart/PinJoint2D");
		_ropeEndPinJoint = GetNode<PinJoint2D>("RopeEnd/PinJoint2D");

		_ropeStart.rope = this;
		_ropeEnd.rope = this;

		SpawnRope();
	}

    public override void _Process(double dt)
    {
        UpdateLine2DRope();
    }


	public void SpawnRope()
	{
		Vector2 ropeStartPos = _ropeStart.Position;
		Vector2 ropeEndPos = _ropeEnd.Position;

		float currentDistance = segmentLength;
		float distance = ropeStartPos.DistanceTo(ropeEndPos);

		Vector2 direction = (ropeEndPos - ropeStartPos).Normalized();
		float rotationAngle = direction.Angle() - Mathf.Pi/2;

		RopeSegment currentSegment = _ropeStart;

		_ropeSegments.Clear();
		_ropeSegments.Add(currentSegment);

		while (currentDistance < distance)
		{
			Vector2 newSegmentPos = ropeStartPos + currentDistance*direction;
			currentSegment = AppendRopeSegment(currentSegment, rotationAngle, newSegmentPos);
			_ropeSegments.Add(currentSegment);
			currentDistance += segmentLength;
		}

		ConnectRopeParts(currentSegment, _ropeEnd);
		_ropeEnd.Rotation = rotationAngle;
		_ropeSegments.Add(_ropeEnd);

		if (staticRopeEnd)
		{
			_ropeEnd.Freeze = true;
		}
	}

	private void ConnectRopeParts(RopeSegment a, RopeSegment b)
	{
		PinJoint2D pinJoint = a.GetNode<PinJoint2D>("PinJoint2D");
		pinJoint.NodeB = b.GetPath();
	}

	public RopeSegment AppendRopeSegment(RopeSegment previousSegment, float rotationAngle, Vector2 position)
	{
		RopeSegment segment = CreateRopeSegment(rotationAngle, position);
		AddChild(segment);
		
		PinJoint2D pinJoint = previousSegment.GetNode<PinJoint2D>("PinJoint2D");
		pinJoint.NodeB = segment.GetPath();
		pinJoint.Bias = 0.99f;
		pinJoint.Softness = 0.003f;

		return segment;
	}

	private RopeSegment CreateRopeSegment(float rotationAngle, Vector2 position)
	{
		RopeSegment segment = _ropeSegmentPackedScene.Instantiate<RopeSegment>();
		segment.Position = position;
		segment.Rotation = rotationAngle;
		segment.rope = this;

		return segment;
	}

	private void UpdateLine2DRope()
	{
		_line2DPoints.Clear();
		_line2DPoints.Add(_ropeStart.Position);

		foreach (RopeSegment segment in _ropeSegments)
		{
			_line2DPoints.Add(segment.Position);
		}

		_line2DPoints.Add(_ropeEnd.Position);
		_line2D.Points = _line2DPoints.ToArray();
	}
}
