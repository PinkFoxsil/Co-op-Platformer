using Godot;

public interface IActionComponent
{
    void Init(Node owner);

    virtual void PrePhysicsUpdate(float dt) {}
    virtual void PhysicsUpdate(float dt) {}
    virtual void PostPhysicsUpdate(float dt) {}
}