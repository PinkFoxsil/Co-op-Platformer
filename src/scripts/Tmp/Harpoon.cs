using Godot;

public partial class Harpoon : RigidBody2D
{
    private Transform2D? _newTransform;
    private Vector2? _newLinearVelocity;

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        if (_newTransform != null)
        {
            state.Transform = (Transform2D) _newTransform;
            _newTransform = null;
            
        }

        if (_newLinearVelocity != null)
        {
            state.LinearVelocity = (Vector2) _newLinearVelocity;
            state.AngularVelocity = 0f;
            _newLinearVelocity = null;
        }
    }

    public void SetPhysicsStateTransform(Transform2D transform)
    {
        GlobalTransform = transform;
        _newTransform = transform;
    }

    public void SetPhysicsStateLinearVelocity(Vector2 linearVelocity)
    {
        _newLinearVelocity = linearVelocity;
    }

    public void Disable()
    {
        ProcessMode = ProcessModeEnum.Disabled;
        Hide();
    }

    public void Enable()
    {
        ProcessMode = ProcessModeEnum.Inherit;
        Show();
        Sleeping = false;
    }
}