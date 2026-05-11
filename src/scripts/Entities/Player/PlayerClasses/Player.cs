using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	private ComponentList _componentList;

	public ComponentList ComponentList => _componentList;

	public bool attackLocked {get; private set;}
	public bool movementLocked {get; private set;}

	public override void _Ready()
	{
		_componentList = new ComponentList(this);
		_componentList.RegisterChildren();
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float) delta;
		
		// [TBD] Determine ordering of processing
		_componentList.PrePhysicsProcess(dt);
		_componentList.PhysicsProcess(dt);

		MoveAndSlide();

		_componentList.PostPhysicsProcess(dt);
	}
}