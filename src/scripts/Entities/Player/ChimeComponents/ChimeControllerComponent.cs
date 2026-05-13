using Godot;
using System;

public partial class ChimeControllerComponent : Node, IComponent
{
    public bool enabled;

	private float _inputX;
	private bool _facingRight;

	private Vector2 _mouseWorldPosition;
	private Vector2 _mouseRelativePosition;
	private Vector2 _mouseDirection;

	private Player _character;

    private DashComponent _dashComponent;
    private BaseMovementComponent _moveComponent;
    private DirectionalAttackComponent _attackComponent;
    private GroundSlamComponent _groundSlamComponent;

	public void Init(Node parentNode)
	{
        _facingRight = true;

		_character = (Player) parentNode;

        _moveComponent = (BaseMovementComponent) _character.ComponentList.GetComponent(typeof(BaseMovementComponent));
        _attackComponent = (DirectionalAttackComponent) _character.ComponentList.GetComponent(typeof(DirectionalAttackComponent));
        _dashComponent = (DashComponent) _character.ComponentList.GetComponent(typeof(DashComponent));
        _groundSlamComponent = (GroundSlamComponent) _character.ComponentList.GetComponent(typeof(GroundSlamComponent));

        UpdateMouseProperties();
        Enable(); // Remove this in production and call after loading scene
	}

    public void Enable()
    {
        enabled = true;

        _dashComponent.dashEnabled = true;
        _moveComponent.movementEnabled = true;
        _attackComponent.attackEnabled = true;
    }

	public void PrePhysicsProcess(float dt)
	{
		if (!enabled)
		{
			return;
		}

		UpdateMouseProperties();

        if (Mathf.Abs(InputSingleton.Instance.inputX) > Mathf.Epsilon)
        {
            _facingRight = InputSingleton.Instance.inputX > 0;
        }

        _attackComponent.attackDirection = DirectionUtility.GetCardinalDirection(_mouseRelativePosition);
        _dashComponent.dashDirection = _facingRight ? 1 : -1;

        _dashComponent.dashEnabled = !_attackComponent.isAttacking;
        _moveComponent.movementEnabled = !_dashComponent.isDashing;
        _attackComponent.attackEnabled = !_dashComponent.isDashing;
	}

    private void UpdateMouseProperties()
    {
        _mouseWorldPosition = _character.GetGlobalMousePosition();
		_mouseRelativePosition = _mouseWorldPosition - _character.Position;
		_mouseDirection = _mouseRelativePosition.Normalized();
    }
}