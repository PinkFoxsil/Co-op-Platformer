using Godot;
using System;

public partial class Player : Sprite2D
{
	float speed = 200.0f;
	float rotationSpeed = 3.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Player is ready!");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Rotation += rotationSpeed * (float)delta;

		Position += new Vector2(
			MathF.Cos(Rotation),
			MathF.Sin(Rotation)
		) * speed * (float)delta;
	}
}
