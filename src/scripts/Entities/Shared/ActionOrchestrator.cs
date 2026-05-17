using Godot;
using System.Collections.Generic;

public partial class ActionOrchestrator : Node
{
    private Node _owner;

    private readonly List<IActionComponent> _components = new();

    private readonly HashSet<string> _tags = new();

    public void Init(Node owner)
    {
        _owner = owner;
        RegisterChildren(this);
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
            RegisterChildren(child); 
        }
    }

    // Updates
    public void PrePhysicsUpdate(float dt)
    {
        foreach (IActionComponent component in _components)
        {
            component.PrePhysicsUpdate(dt);
        }
    }

    public void PhysicsUpdate(float dt)
    {
        foreach (IActionComponent component in _components)
        {
            component.PhysicsUpdate(dt);
        }
    }

    public void PostPhysicsUpdate(float dt)
    {
        foreach (IActionComponent component in _components)
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