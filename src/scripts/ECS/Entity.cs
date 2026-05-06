using Godot;
using System;
using System.Collections.Generic;

public class Entity 
{
	public int Id { get; private set; }

	private Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();

	public Entity(int id)
	{
		Id = id;
	}

	public void AddComponent(IComponent component)
	{
		components[component.GetType()] = component;
	}

	public T GetComponent<T>() where T : class, IComponent
	{
		IComponent c;

		if (components.TryGetValue(typeof(T), out c))
		{
			return c as T;
		}
		else
		{
			return null;
		}
	}

	public bool HasComponent<T>() where T : IComponent
	{
		return components.ContainsKey(typeof(T));
	}
}
