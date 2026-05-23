using Godot;
using System.Collections.Generic;

public partial class ActionOrchestrator : Node
{
	private Node _owner;

	private readonly List<IActionComponent> _components = new();
	private readonly List<IActionComponent> _prePhysics = new();
	private readonly List<IActionComponent> _physics = new();
	private readonly List<IActionComponent> _postPhysics = new();

	private readonly HashSet<string> _tags = new();

	public void Init(Node owner)
	{
		_owner = owner;
		RegisterChildren(this);
		BuildSortedLists();
	}

	private void RegisterChildren(Node node) 
	{
		foreach (Node child in node.GetChildren()) 
		{ 
			if (child is IActionComponent component)
			{ 
				component.Init(_owner); 
				_components.Add(component); 
			}
		}
	}

	private void BuildSortedLists()
	{
		_prePhysics.Clear();
		_physics.Clear();
		_postPhysics.Clear();

		_prePhysics.AddRange(_components);
		_physics.AddRange(_components);
		_postPhysics.AddRange(_components);

		_prePhysics.Sort((a, b) => a.PrePhysicsPriority.CompareTo(b.PrePhysicsPriority));
		_physics.Sort((a, b) => a.PhysicsPriority.CompareTo(b.PhysicsPriority));
		_postPhysics.Sort((a, b) => a.PostPhysicsPriority.CompareTo(b.PostPhysicsPriority));
	}

	public void RegisterComponent(IActionComponent component)
	{
		if (_components.Contains(component))
			return;

		component.Init(_owner);
		_components.Add(component);

		InsertSorted(_prePhysics, component, (a, b) => a.PrePhysicsPriority.CompareTo(b.PrePhysicsPriority));
		InsertSorted(_physics, component, (a, b) => a.PhysicsPriority.CompareTo(b.PhysicsPriority));
		InsertSorted(_postPhysics, component, (a, b) => a.PostPhysicsPriority.CompareTo(b.PostPhysicsPriority));
	}

	private void InsertSorted(List<IActionComponent> list, IActionComponent item, System.Comparison<IActionComponent> comparison)
	{
		int index = list.FindIndex(x => comparison(item, x) < 0);

		if (index < 0)
		{
			list.Add(item);
		}
		else
		{
			list.Insert(index, item);
		}
	}

	// Updates
	public void PrePhysicsUpdate(float dt)
	{
		foreach (IActionComponent component in _prePhysics)
		{
			component.PrePhysicsUpdate(dt);
		}
	}

	public void PhysicsUpdate(float dt)
	{
		foreach (IActionComponent component in _physics)
		{
			component.PhysicsUpdate(dt);
		}
	}

	public void PostPhysicsUpdate(float dt)
	{
		foreach (IActionComponent component in _postPhysics)
		{
			component.PostPhysicsUpdate(dt);
		}
	}

	// Tags
	public void AddTag(string tag)
	{
		_tags.Add(tag);
	}

	public void RemoveTag(string tag)
	{
		_tags.Remove(tag);
	}

	public bool HasTag(string tag)
	{
		return _tags.Contains(tag);
	}

	// Permissions
	public bool CanMove()
	{
		return !HasTag("MovementLocked");
	}

	public bool CanAttack()
	{
		return !HasTag("AttackLocked");
	}

	public bool CanDash()
	{
		return !HasTag("DashLocked");
	}
}
