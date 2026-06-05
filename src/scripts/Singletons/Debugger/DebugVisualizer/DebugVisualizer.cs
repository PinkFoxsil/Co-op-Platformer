using Godot;
using System.Collections.Generic;

public partial class DebugVisualizer : Node2D
{
    private List<DebugVisual> _visuals = [];

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

        _visuals.Add(circleVisual);
    }

    public void DrawVector(Vector2 position, Vector2 direction, float width = 2.5f, Color? color = null, float duration = -1f)
    {
        DebugVectorVisual vectorVisual = new(
            position,
            direction,
            width,
            color,
            duration
        );

        _visuals.Add(vectorVisual);
    }

    private void DrawVisuals()
    {
        foreach (DebugVisual visual in _visuals)
        {
            visual.Draw(this);
        }
    }

    private void UpdateLifetimes(float delta)
    {
        for (int i = _visuals.Count - 1; i >= 0; i--)
        {
            DebugVisual visual = _visuals[i];

            if (visual.Duration < 0f)
            {
                visual.Duration = 0f;
                continue;
            }

            visual.ElapsedTime += delta;
            if (visual.ElapsedTime > visual.Duration)
            {
                _visuals.RemoveAt(i);
            }
        }
    }
}