using Godot;

public abstract class DebugVisual
{
    public float ElapsedTime = 0f;

    public float Duration;
    public Vector2 Position;
    public Color Color;

    public DebugVisual(Vector2 position, Color? color = null, float duration = -1f)
    {
        Position = position;
        Color = color ?? Colors.White;
        Duration = duration;
    }

    public void Update(float delta)
    {
        ElapsedTime += delta;
    }

    public abstract void Draw(CanvasItem canvasItem);
}