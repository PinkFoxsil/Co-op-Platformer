using Godot;
using System;
using System.Collections.Generic;

public abstract class SystemBase
{
	public abstract void Update(List<Entity> entities, float delta);
}
