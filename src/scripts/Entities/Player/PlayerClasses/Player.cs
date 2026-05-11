using Godot;
using System;
using System.Collections.Generic;
using System.Data;

public partial class Player : CharacterBody2D
{

	private ComponentList _componentList;

	public ComponentList ComponentList => _componentList;

	public PlayerState currentState;

	public override void _Ready()
	{
		_componentList = new ComponentList(this);
		_componentList.RegisterChildren();

		currentState = new PlayerState();
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
