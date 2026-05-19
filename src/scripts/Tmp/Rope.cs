using Godot;
using System;
using System.Collections.Generic;

public partial class Rope : Node2D
{
	[Export] public PinJoint2D startPinJoint;
	[Export] public PinJoint2D entPinJoint;
	[Export] public float segmentLength = 20f;
	[Export] public float softness = 0.003f;

	// When true set the length every physics process.
	public bool dynamicLength;

	private Line2D _line2D;
	private List<Vector2> _line2DPoints = [];
	
	private PackedScene _ropeSegmentPackedScene;
	private LinkedList<RopeSegment> _ropeSegments = [];

	private float _halfSegmentLength;

    public override void _Ready()
	{
		_ropeSegmentPackedScene = GD.Load<PackedScene>("res://src/scenes/rope_segment.tscn");
		
		_line2D = GetNode<Line2D>("Line2D");

		_halfSegmentLength = segmentLength * 0.5f;

		SpawnRope();
	}

    public override void _Process(double dt)
    {
        UpdateLine2DRope();
    }

	public void SpawnRope()
	{
		Vector2 startPos = ToLocal(startPinJoint.GlobalPosition);
		Vector2 endPos = ToLocal(entPinJoint.GlobalPosition);

		Vector2 startToEndVect = endPos - startPos;
		float distance = startToEndVect.Length();
		Vector2 direction = startToEndVect.Normalized();

		float rotation = direction.Angle() - Mathf.Pi / 2;

		float targetDistance = distance - _halfSegmentLength;
		for (float interval = _halfSegmentLength; interval <= targetDistance; interval += segmentLength)
		{
			Vector2 newSegmentPos = interval * direction + startPos;
			RopeSegment newSegment = CreateRopeSegment(newSegmentPos, rotation);
			AppendRopeSegment(newSegment);
		}

		if (_ropeSegments.Last != null)
		{
			_ropeSegments.Last.Value.pinJoint.QueueFree();
			entPinJoint.NodeA = _ropeSegments.Last.Value.GetPath();
			_ropeSegments.Last.Value.pinJoint = entPinJoint;
		}
	}

	public void AppendRopeSegment(RopeSegment segment)
	{
		if (_ropeSegments.Last != null)
		{
			PinJoint2D pinJoint = _ropeSegments.Last.Value.pinJoint;
			pinJoint.NodeB = segment.GetPath();
		}
		else
		{
			startPinJoint.NodeB = segment.GetPath();
		}

		_ropeSegments.AddLast(segment);
		
	}

	private RopeSegment CreateRopeSegment(Vector2 position, float rotation)
	{
		RopeSegment segment = _ropeSegmentPackedScene.Instantiate<RopeSegment>();
		segment.Position = position;
		segment.Rotation = rotation;
		segment.length = segmentLength;

		AddChild(segment);
		PinJoint2D pinJoint = segment.pinJoint;
		pinJoint.Softness = softness;

		return segment;
	}

	private void UpdateLine2DRope()
	{
		_line2DPoints.Clear();
		_line2DPoints.Add(ToLocal(startPinJoint.GlobalPosition));

		foreach (RopeSegment segment in _ropeSegments)
		{
			_line2DPoints.Add(ToLocal(segment.pinJoint.GlobalPosition));
		}

		_line2D.Points = _line2DPoints.ToArray();
	}
}
