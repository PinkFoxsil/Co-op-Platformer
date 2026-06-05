using System;
using Godot;

public class DebugVectorVisual : DebugVisual
{
    public Vector2 Direction;
    public float Width;

    public DebugVectorVisual(Vector2 position, Vector2 direction, float width = 2.5f, Color? color = null, float duration = -1f) : base(position, color, duration)
    {
        Direction = direction;
        Width = width;
    }

    public override void Draw(CanvasItem canvasItem)
    {
        // Draw Line
        Vector2 vectorGoalPosition = Position + Direction;
        Vector2 directionUnitVector = Direction.Normalized();
        Vector2 lineEndPosition = vectorGoalPosition - directionUnitVector * Width * 4f;

        canvasItem.DrawLine(Position, lineEndPosition, Color, Width);

        // Draw Triangle
        Vector2 perpendicularUnitVector = new(directionUnitVector.Y, -directionUnitVector.X);
        
        Vector2[] points =
        [
            lineEndPosition + perpendicularUnitVector * Width * 2f,
            lineEndPosition - perpendicularUnitVector * Width * 2f,
            vectorGoalPosition,
        ];

        Color[] colors = new Color[3];
        Array.Fill(colors, Color);

        canvasItem.DrawPolygon(points, colors);
    }
}