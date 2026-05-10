using Godot;
using System;

public abstract partial class Component : Node
{
	public Node ParentNode { get; private set; }

	public virtual void Init(Node parentNode)
	{
			ParentNode = parentNode;
	}

	public virtual void PrePhysicsProcess(float dt) { }
	public virtual void PhysicsProcess(float dt) { }
	public virtual void PostPhysicsProcess(float dt) { }
}
