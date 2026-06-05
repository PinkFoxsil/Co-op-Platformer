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

    public Vector2 TailPosition => ToGlobal(GetLengthOffset(0f));
    public Vector2 HeadPosition => ToGlobal(GetLengthOffset(1f));

    private Vector2 _targetPosition;
    private bool _forceSetPosition = false;

    private CollisionShape2D _collisionShape;
    private CapsuleShape2D _capsuleShape;

    private float _diameter;
    private float _halfLength;

    public RopeSegment()
    {
        _capsuleShape = CreateCapsuleShape();
        _collisionShape = new()
		{
            Name = "CollisionShape2D",
            Rotation = Mathf.DegToRad(90.0f),
			Shape = _capsuleShape
		};

        CustomIntegrator = true;
        
        AddChild(_collisionShape);
    }

    public Vector2 GetLengthOffset(float scale)
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