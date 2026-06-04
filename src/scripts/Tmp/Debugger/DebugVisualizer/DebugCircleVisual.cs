using Godot;

public class DebugCircleVisual : DebugVisual
{
    public float Radius;

    public DebugCircleVisual(Vector2 position, float radius = 2.5f, Color? color = null, float duration = -1f) : base(position, color, duration)
    {
        Radius = radius;
    }

    public override void Draw(CanvasItem canvasItem)
    {
        canvasItem.DrawCircle(Position, Radius, Color);
    }
}