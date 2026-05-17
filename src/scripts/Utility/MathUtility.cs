using Godot;
using System;

public static class MathUtility
{
	public static float SnapToZero(float value, float threshold = Mathf.Epsilon)
	{
		return Mathf.Abs(value) < threshold ? 0f : value;
	}
}