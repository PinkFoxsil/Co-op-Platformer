using Godot;

public partial class RopeSegment : RigidBody2D
{
    public float Length {
        get;
        set
        {
            field = value;
            _halfLength = value * 0.5f;
            UpdateCapsuleHeight();
        }
    }

    public float Width {
        get;
        set
        {
            field = value;
            _diameter = Width * 2.0f;
            UpdateCapsuleHeight();
            UpdateCapsuleRadius();
        }
    }

    public Vector2 TailOffset => GetLengthOffset(0f);
    public Vector2 HeadOffset => GetLengthOffset(1f);
    public Vector2 TailPosition => ToGlobal(TailOffset);
    public Vector2 HeadPosition => ToGlobal(HeadOffset);

    public PinJoint2D PinJoint
    {
        get;
        set
        {
            field = value;
        }
    }

    private CollisionShape2D _collisionShape;
    private CapsuleShape2D _capsuleShape;

    private float _diameter;
    private float _halfLength;
    private Transform2D? _newTransform;

    public override void _Ready()
    {
        _capsuleShape = CreateCapsuleShape();
        _collisionShape = CreateCollisionShape();
        
        CallDeferred(Node.MethodName.AddChild, _collisionShape);
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        if (_newTransform != null)
        {
            state.Transform = (Transform2D)_newTransform;
            _newTransform = null;
        }
    }

    public Vector2 GetLengthOffset(float scale)
    {
        return new(Length * scale - _halfLength, 0);
    }

    public void SetPhysicsStateTransform(Transform2D transform)
    {
        GlobalTransform = transform;
        _newTransform = transform;
    }

    public void ConnectTo(CollisionObject2D other, float softness, float bias)
    {
        PinJoint = new()
		{
			Name = "PinJoint",
			Position = HeadOffset,
			Softness = softness,
			Bias = bias,
			NodeA = GetPath(),
			NodeB = other.GetPath()
		};

        AddChild(PinJoint);

        return;
    }

    // Resets the pin joints collision so the offset from repositioning it is correct
    public void UpdatePinJointAnchorOffset()
    {
        PinJoint.DisableCollision = !PinJoint.DisableCollision;
		PinJoint.DisableCollision = !PinJoint.DisableCollision;
    }

    private void UpdateCapsuleHeight()
    {
        if (_capsuleShape == null)
        {
            return;
        }

        _capsuleShape.Height = _diameter + Length;
    }

    private void UpdateCapsuleRadius()
    {
        if (_capsuleShape == null)
        {
            return;
        }

        _capsuleShape.Radius = Width;
    }

	private CapsuleShape2D CreateCapsuleShape()
	{
		return new()
		{
			Radius = Width,
			Height = _diameter + Length
		};
	}

    private CollisionShape2D CreateCollisionShape()
    {
        return new()
		{
            Name = "CollisionShape2D",
            Rotation = Mathf.DegToRad(90.0f),
			Shape = _capsuleShape
		};
    }
}