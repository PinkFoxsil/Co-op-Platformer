using Godot;
using System;
using System.Collections.Generic;

public class Entity<T> where T : Node
{
	private readonly T _node;

	public T node => _node;

	private readonly List<Component<T>> _components = new();

	public Entity(T entity)
	{
		_node = entity;
	}

	public void RegisterChildren()
	{
		foreach (Node child in _node.GetChildren())
		{
			if (child is Component<T> c)
			{
				AddComponent(c);
			}
		}
	}

	public void AddComponent(Component<T> component)
	{
		_components.Add(component);
		component.Init(this);
	}

	public U GetComponent<U>() where U : Component<T>
	{
		foreach (var c in _components)
		{
			if (c is U u)
				return u;
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