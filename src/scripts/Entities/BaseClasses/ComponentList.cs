using Godot;
using System;
using System.Collections.Generic;

public class ComponentList
{
	private readonly Node _parentNode;

	public Node ParentNode => _parentNode;

	private readonly List<Component> _components = [];

	private ComponentList(){}

	public ComponentList(Node parentNode)
	{
		_parentNode = parentNode;
	}

	public void RegisterChildren()
	{
		foreach (Node child in _parentNode.GetChildren())
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
		component.Init(_parentNode);
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
