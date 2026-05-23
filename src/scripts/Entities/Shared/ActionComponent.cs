using Godot;

public interface IActionComponent
{
    int PrePhysicsPriority => 0;
    int PhysicsPriority => 0;
    int PostPhysicsPriority => 0;
    
    void Init(Node owner);

    virtual void PrePhysicsUpdate(float dt) {}
    virtual void PhysicsUpdate(float dt) {}
    virtual void PostPhysicsUpdate(float dt) {}
}