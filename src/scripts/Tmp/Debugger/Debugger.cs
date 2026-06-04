using Godot;

public partial class Debugger : Node
{
    public static Debugger Instance { get; private set; }
    public Node CurrentScene { get; set; }

    private DebugVisualizer _visualizer;

    private int _physicsStepCount = -1;

    public override void _Ready()
    {
        _visualizer = new DebugVisualizer();
        Viewport root = GetTree().Root;
        CurrentScene = root.GetChild(-1);

        CurrentScene.AddChild(_visualizer);

        Instance = this;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_physicsStepCount < 0)
        {
            return;
        }

        if (_physicsStepCount == 0)
        {
            GetTree().Paused = true;
        }
        else
        {
            GetTree().Paused = false;
        }

        _physicsStepCount--;
    }

    // -1 acts as 1 frame
    public void DrawPoint(Vector2 position, Color color, float duration = -1f)
    {
        _visualizer.DrawPoint(position, color, duration);
    }

    public void Pause()
    {
        GetTree().Paused = true;
        _physicsStepCount = 0;
    }

    public void Play()
    {
        GetTree().Paused = false;
        _physicsStepCount = -1;
    }

    public void StepPhysics()
    {
        _physicsStepCount = 1;
    }
}