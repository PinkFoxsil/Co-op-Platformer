using Godot;
using System;
using System.Collections.Generic;

public class Entity
{
	private readonly Node _node;

	public Node node => _node;

	private readonly List<Component> _components = new();

	public Entity(Node entity)
	{
		_node = entity;
	}

	public void RegisterChildren()
	{
		foreach (Node child in _node.GetChildren())
		{
			if (child is Component c)
			{
				AddComponent(c);
			}
		}
	}

	public void AddComponent(Component component)
	{
		_components.Add(component);
		component.Init(this);
	}

	public Component GetComponent(Type type)
	{
		foreach (var c in _components)
		{
			if (type.IsInstanceOfType(c))
				return c;
		}

		return null;
	}

	public void PrePhysicsProcess(float dt)
	{
		foreach (var c in _components)
		{
			c.PrePhysicsProcess(dt);
		}
	}

	public void PhysicsProcess(float dt)
	{
		foreach (var c in _components)
		{
			c.PhysicsProcess(dt);
		}
	}

	public void PostPhysicsProcess(float dt)
	{
		foreach (var c in _components)
		{
			c.PostPhysicsProcess(dt);
		}
	}
}
