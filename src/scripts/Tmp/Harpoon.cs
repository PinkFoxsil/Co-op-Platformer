using Godot;

public partial class Harpoon : Projectile
{
    public override void OnHit(ShapeCastCollision collision)
    {
        Position = collision.Point;
        Rotation = Velocity.Angle() - Mathf.Pi * 0.5f;
        Active = false;
    }
}