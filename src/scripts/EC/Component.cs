using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Component<T> : Node where T : Node
{
	public Entity<T> entity { get; private set; }

	public virtual void Init(Entity<T> entity)
	{
			this.entity = entity;
	}

	public virtual void PrePhysicsProcess(float dt) { }
	public virtual void PhysicsProcess(float dt) { }
	public virtual void PostPhysicsProcess(float dt) { }
}
