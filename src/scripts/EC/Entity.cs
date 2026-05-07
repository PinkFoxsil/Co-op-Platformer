using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Entity : CharacterBody2D
{
	protected List<Component> components = new();

	public override void _Ready()
  	{
		foreach(Node child in GetChildren())
		{
			if (child is Component c) {
				AddComponent(c);
			}
		}
	}

	public void AddComponent(Component component)
	{
		components.Add(component);
		component.Init(this);
	}

	public T GetComponent<T>() where T : Component
	{
			foreach (var c in components)
					if (c is T t) return t;
			return null;
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float) delta;
		
		foreach(var c in components)
		{
			c.PrePhysicsProcess(dt);
		}

		foreach(var c in components)
		{
			c.PhysicsProcess(dt);
		}

		MoveAndSlide();

		foreach(var c in components)
		{
			c.PostPhysicsProcess(dt);
		}
	}
}
