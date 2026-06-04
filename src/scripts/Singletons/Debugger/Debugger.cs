using Godot;

public partial class Debugger : Node
{
    public static Debugger Instance { get; private set; }
    public Node CurrentScene { get; set; }

    private DebugVisualizer _visualizer;

    private bool _debugSimulationActive = false;
    private int _physicsStepCount = -1;

    public override void _Ready()
    {
        _visualizer = new DebugVisualizer();

        Viewport root = GetTree().Root;
        CurrentScene = root.GetChild(-1);
        CurrentScene.AddChild(_visualizer);

        ProcessMode = ProcessModeEnum.Always;

        Instance = this;
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if (!_debugSimulationActive)
        {
            return;
        }

        if (inputEvent is InputEventKey eventKey)
        {
            if (eventKey.Pressed && eventKey.Keycode == Key.Period)
            {
                StepPhysics();
            }
        }
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
            return;
        }
        
        GetTree().Paused = false;
        _physicsStepCount--;
    }

    // -1 acts as 1 frame
    public void DrawCircle(Vector2 position, float radius = 2.5f, Color? color = null, float duration = -1f)
    {
        _visualizer.DrawCircle(position, radius, color, duration);
    }

    public void StartDebugSimulation()
    {
        if (_debugSimulationActive)
        {
            return;
        }

        GetTree().Paused = true;
        _debugSimulationActive = true;
        _physicsStepCount = 0;
    }

    public void StopDebugSimulation()
    {
        if (!_debugSimulationActive)
        {
            return;
        }

        GetTree().Paused = false;
        _debugSimulationActive = false;
        _physicsStepCount = -1;
    }

    public void StepPhysics()
    {
        _physicsStepCount = 1;
    }
}