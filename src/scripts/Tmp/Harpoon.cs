using Godot;

public partial class Harpoon : Projectile
{
    public Marker2D ropeAttachMarker;

    public override void _Ready()
    {
        base._Ready();

        Name = "Harpoon";
        ropeAttachMarker = GetNode<Marker2D>("RopeAttachMarker");
    }

    public override void Hit(float dt, ShapeCastCollision collision)
    {
        shapeCast.Hide();
        Position += Velocity * dt * shapeCast.GetClosestCollisionSafeFraction();
        Rotation = Velocity.Angle();
        Active = false;

        EmitSignal(Projectile.SignalName.OnHit);
    }
}