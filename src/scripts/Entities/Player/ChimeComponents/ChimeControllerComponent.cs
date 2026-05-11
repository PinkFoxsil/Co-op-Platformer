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

        _dashComponent.canDash = true;
        _moveComponent.canMove = true;
        _attackComponent.canAttack = true;
    }

	public void PrePhysicsProcess(float dt)
	{
		if (!enabled)
		{
			return;
		}

		UpdateMouseProperties();

        _attackComponent.attackDirection = DirectionUtility.GetCardinalDirection(_mouseRelativePosition);

        _dashComponent.canDash = true;
        _moveComponent.canMove = true;
        _attackComponent.canAttack = true;

        if (_attackComponent.isAttacking)
        {
            _attackComponent.canAttack = false;
            _dashComponent.canDash = false;
        }
        
        if (_dashComponent.isDashing)
        {
            _dashComponent.canDash = false;
            _attackComponent.canAttack = false;
            _moveComponent.canMove = false;
        }
	}

    private void UpdateMouseProperties()
    {
        _mouseWorldPosition = _character.GetGlobalMousePosition();
		_mouseRelativePosition = _mouseWorldPosition - _character.Position;
		_mouseDirection = _mouseRelativePosition.Normalized();
    }
}