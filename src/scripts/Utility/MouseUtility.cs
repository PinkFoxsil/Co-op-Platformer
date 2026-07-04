using Godot;

public static class MouseUtility
{
    // This can be moved to either a helper module or input component
    public static Vector2 GetMouseUnitVector(Node2D node)
    {
        return GetMouseVector(node).Normalized();
    }

    // This can be moved to either a helper module or input component
    public static Vector2 GetMouseVector(Node2D node)
    {
        Vector2 mousePosition = node.GetGlobalMousePosition();
        return mousePosition - node.GlobalPosition;
    }
}