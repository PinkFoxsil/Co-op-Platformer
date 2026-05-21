using Godot;

public partial class Harpoon : Projectile
{
    public override void OnHit(float dt, ShapeCastCollision collision)
    {
        shapeCast.Hide();
        Position += Velocity * dt * shapeCast.GetClosestCollisionSafeFraction();
        Rotation = Velocity.Angle();
        Active = false;
    }
}