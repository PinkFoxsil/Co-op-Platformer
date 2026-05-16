using Godot;
using System;

public partial class ActionComponent : CharacterBody2D
{
    public PlayerInput Input { get; private set; }
    public CharacterMotor Motor { get; private set; }
    public ActionOrchestrator Orchestrator { get; private set; }

    public override void _Ready()
    {
        Input = GetNode<PlayerInput>("PlayerInput");
        Motor = GetNode<CharacterMotor>("CharacterMotor");
        Orchestrator = GetNode<ActionOrchestrator>("ActionOrchestrator");

        Input.Init(this);
        Motor.Init(this);
        Orchestrator.Init(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float) delta;
        Input.Capture();
        Orchestrator.PrePhysicsUpdate(dt);
        Motor.Resolve(dt);

        MoveAndSlide();

        Orchestrator.PostPhysicsUpdate(dt);
    }
}