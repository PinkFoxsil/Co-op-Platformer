using Godot;
using System.Collections.Generic;

public partial class DebugVisualizer : Node2D
{
    private List<DebugCircleVisual> _pointVisuals = [];

    public override void _Ready()
    {
        Name = "DebugVisualizer";
    }

    public override void _Process(double delta)
    {
        UpdateLifetimes((float) delta);
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawVisuals();
    }

    public void DrawCircle(Vector2 position, float radius = 2.5f, Color? color = null, float duration = -1f)
    {
        DebugCircleVisual circleVisual = new(
            position,
            radius,
            color,
            duration
        );

        _pointVisuals.Add(circleVisual);
    }

    private void DrawVisuals()
    {
        foreach (DebugCircleVisual pointVisual in _pointVisuals)
        {
            pointVisual.Draw(this);
        }
    }

    private void UpdateLifetimes(float delta)
    {
        for (int i = _pointVisuals.Count - 1; i >= 0; i--)
        {
            DebugCircleVisual pointVisual = _pointVisuals[i];

            if (pointVisual.Duration < 0f)
            {
                pointVisual.Duration = 0f;
                continue;
            }

            pointVisual.ElapsedTime += delta;
            if (pointVisual.ElapsedTime > pointVisual.Duration)
            {
                _pointVisuals.RemoveAt(i);
            }
        }
    }
}