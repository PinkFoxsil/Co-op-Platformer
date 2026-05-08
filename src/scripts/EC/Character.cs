using Godot;
using System;

public partial class Character : CharacterBody2D
{
    private Entity<CharacterBody2D> _entity;

    public Entity<CharacterBody2D> entity => _entity;

	public override void _Ready()
	{
		_entity = new Entity<CharacterBody2D>(this);
		_entity.RegisterChildren();
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float) delta;
		
		_entity.PrePhysicsProcess(dt);
		_entity.PhysicsProcess(dt);

		MoveAndSlide();

		_entity.PostPhysicsProcess(dt);
	}
}