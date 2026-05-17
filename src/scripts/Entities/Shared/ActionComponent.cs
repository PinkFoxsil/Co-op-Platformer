public interface IActionComponent
{
    void Init(Player player);

    virtual void PrePhysicsUpdate(float dt) {}
    virtual void PhysicsUpdate(float dt) {}
    virtual void PostPhysicsUpdate(float dt) {}
}