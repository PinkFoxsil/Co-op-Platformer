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

    private CollisionShape2D _collisionShape;
    private CapsuleShape2D _capsuleShape;

    private float _diameter;
    private float _halfLength;

    public RopeSegment()
    {
        _capsuleShape = CreateCapsuleShape();
        _collisionShape = new()
		{
            Rotation = Mathf.DegToRad(90.0f),
			Shape = _capsuleShape
		};
        
        AddChild(_collisionShape);
    }

    public Vector2 GetPositionAlongLength(float scale)
    {
        return new(Length * scale - _halfLength, 0);
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
}