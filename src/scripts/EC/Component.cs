using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Component : Node
{
	public Entity entity { get; private set; }

	public void Init(Entity entity)
	{
			this.entity = entity;
			GD.Print(this.entity);
	}

	public virtual void PrePhysicsProcess(float dt) { }
	public virtual void PhysicsProcess(float dt) { }
	public virtual void PostPhysicsProcess(float dt) { }
}
