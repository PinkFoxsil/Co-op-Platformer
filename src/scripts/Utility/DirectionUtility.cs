using Godot;
using System;

public enum CardinalDirection
{
    CENTER,
    UP,
    DOWN,
    LEFT,
    RIGHT,
}

public enum CompassDirection
{
    CENTER,
    UP,
    DOWN,
    LEFT,
    RIGHT,
    UP_LEFT,
    UP_RIGHT,
    DOWN_LEFT,
    DOWN_RIGHT
}

public static class DirectionUtility
{
    public static readonly Vector2 CENTER = Vector2.Zero;
    
    public static readonly Vector2 UP = Vector2.Up;
    public static readonly Vector2 DOWN = Vector2.Down;
    public static readonly Vector2 LEFT = Vector2.Left;
    public static readonly Vector2 RIGHT = Vector2.Right;

    public static readonly Vector2 UP_LEFT = new Vector2(-1, -1).Normalized();
    public static readonly Vector2 UP_RIGHT = new Vector2(1, -1).Normalized();
    public static readonly Vector2 DOWN_LEFT = new Vector2(-1, 1).Normalized();
    public static readonly Vector2 DOWN_RIGHT = new Vector2(1, 1).Normalized();

    public static Vector2 ToVector(CardinalDirection dir)
    {
        return dir switch
        {
            CardinalDirection.UP => UP,
            CardinalDirection.DOWN => DOWN,
            CardinalDirection.LEFT => LEFT,
            CardinalDirection.RIGHT => RIGHT,
            _ => CENTER,
        };

    }

    public static Vector2 ToVector(CompassDirection dir)
    {
        return dir switch
        {
            CompassDirection.UP => UP,
            CompassDirection.DOWN => DOWN,
            CompassDirection.LEFT => LEFT,
            CompassDirection.RIGHT => RIGHT,
            CompassDirection.UP_LEFT => UP_LEFT,
            CompassDirection.UP_RIGHT => UP_RIGHT,
            CompassDirection.DOWN_LEFT => DOWN_LEFT,
            CompassDirection.DOWN_RIGHT => DOWN_RIGHT,
            _ => CENTER,
        };

    }
        
    public static CardinalDirection GetCardinalDirection(Vector2 origin, Vector2 destination)
    {

        Vector2 dir = destination - origin;
        return GetCardinalDirection(dir);
    
    }

    public static CardinalDirection GetCardinalDirection(Vector2 dir)
    {
        if (dir == Vector2.Zero)
        {
            return CardinalDirection.CENTER;
        }

        dir = dir.Normalized();
        if (Mathf.Abs(dir.X) > Mathf.Abs(dir.Y))
        {
            return dir.X > 0
                ? CardinalDirection.RIGHT
                : CardinalDirection.LEFT;
        }

        return dir.Y > 0
            ? CardinalDirection.DOWN
            : CardinalDirection.UP;
    }

    public static CompassDirection GetCompassDirection(Vector2 origin, Vector2 destination)
    {

        Vector2 dir = destination - origin;
        return GetCompassDirection(dir);
    
    }

    public static CompassDirection GetCompassDirection(Vector2 dir)
    {
        if (dir == Vector2.Zero)
        {
            return CompassDirection.CENTER;
        }

        float angle = Mathf.RadToDeg(dir.Angle());
        angle = (angle + 360) % 360;

        if (angle >= 337.5f || angle < 22.5f)
        {
            return CompassDirection.RIGHT;
        }

        if (angle < 67.5f)
        {
            return CompassDirection.DOWN_RIGHT;
        }

        if (angle < 112.5f)
        {
            return CompassDirection.DOWN;
        }

        if (angle < 157.5f)
        {
            return CompassDirection.DOWN_LEFT;
        }

        if (angle < 202.5f)
        {
            return CompassDirection.LEFT;
        }

        if (angle < 247.5f)
        {
            return CompassDirection.UP_LEFT;
        }

        if (angle < 292.5f)
        {
            return CompassDirection.UP;
        }

        return CompassDirection.UP_RIGHT;
    }
}