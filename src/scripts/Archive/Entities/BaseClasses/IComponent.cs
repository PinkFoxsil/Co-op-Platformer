using Godot;
using System;

public interface IComponent
{
	public virtual void Init(Node parentNode) { }

	public virtual void PrePhysicsProcess(float dt) { }
	public virtual void PhysicsProcess(float dt) { }
	public virtual void PostPhysicsProcess(float dt) { }
}