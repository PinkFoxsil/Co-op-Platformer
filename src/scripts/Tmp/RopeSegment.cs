using Godot;
using System;

public partial class RopeSegment : RigidBody2D
{
	public PinJoint2D pinJoint;

    public override void _Ready()
    {
        pinJoint = GetNode<PinJoint2D>("PinJoint2D");
    }
}
