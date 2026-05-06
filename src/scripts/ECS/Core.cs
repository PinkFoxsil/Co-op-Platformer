using Godot;
using System;
using System.Collections.Generic;

public class Entity
{
	public int Id { get; private set; }
	private Dictionary<Type, IComponent> components = new();

	public Entity(int id) => Id = id;

	public void AddComponent(IComponent component)
		=> components[component.GetType()] = component;

	public T GetComponent<T>() where T : class, IComponent
		=> components.TryGetValue(typeof(T), out var c) ? c as T : null;

	public bool HasComponent<T>() where T : IComponent
		=> components.ContainsKey(typeof(T));
}

public interface IComponent { }

public abstract class SystemBase
{
	public abstract void Update(List<Entity> entities, float delta);
}
