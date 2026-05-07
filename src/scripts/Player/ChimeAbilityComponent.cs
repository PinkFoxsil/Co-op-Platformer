using Godot;
using System;

public partial class ChimeAbilityComponent : Component
{
    [Export] public PackedScene chimelingScene;

	[Export] public int maxChimelings = 2;

    [Export] public float maxHoldTime = 3f;

    [Export] public float holdCooldown = 5f;
    [Export] public float tapCooldown = 1.5f;

    [Export] public float chimelingSpeed = 600f;

    private float _holdTimer;

    private float _holdCooldownTimer;
    private float _tapCooldownTimer;

    private bool _abilityActive;
    private bool _requiresGroundReset;

    private List<Chimeling> _chimelingPool = new();
    private int _chimelingIndex = 0

    private InputComponent _input;

    public override void Ready()
    {
        _input = entity.GetComponent<InputComponent>();

       UpdateChimeLingPoolSize();
    }

    private void UpdateChimeLingPoolSize()
    {
        while (_chimelingPool.Count < maxChimelings)
        {
            Chimeling chimeling = chimelingScene.Instantiate<Chimeling>();

            this.AddChild(chimeling);

            chimeling.Deactivate();

            _chimelingPool.Add(chimeling);
        }
    }

    public void UnlockNewChimeling()
    {
        maxChimelings++;
        UpdateChimelingPool();
    }

    public override void PrePhysicsProcess(float dt)
    {
        _holdCooldownTimer -= dt;
        _tapCooldownTimer -= dt;

        HandleGroundReset();
        HandleHoldState(dt);
        HandleTap();

    }

    private void HandleGroundReset()
    {
        if (_requiresGroundReset && entity.IsOnFloor())
        {
            _requiresGroundReset = false;
        }
    }

    private void HandleHoldState(float dt)
    {

        // Begin holding
        if (_input.attack2Held && !_abilityActive)
        {
            if (_holdCooldownTimer > 0)
                return;

            if (_requiresGroundReset)
                return;

            BeginHold();
        }

        // Update holding
        if (_abilityActive)
        {
            _holdTimer -= dt;

            entity.Velocity = Vector2.Zero;

            if (_holdTimer <= 0)
            {
                EndHold();
                return;
            }

            // Manual release
            if (!_input.attack2Released)
            {
                EndHold();
                return;
            }
        }
    }

    private void BeginHold()
    {
        _abilityActive = true;

        _holdTimer = maxHoldTime;

        if (_chimelingIndex < _chimelingPool.count) {
            Chimeling chimeling = _chimelingPool[_chimelingIndex];
            _chimelingIndex++
            chimeling.Activate();

            chimeling.GlobalPosition = entity.GlobalPosition;
            NoteDirection dir = GetQuadrant(_input.mouseDirection);

            chimeling.MoveTo(_input.mouseWorldPosition, chimelingSpeed);
        }
    }

    private NoteDirection GetQuadrant(Vector2 dir)
    {
        if (Mathf.Abs(dir.X) > Mathf.Abs(dir.Y))
        {
            return dir.X > 0
                ? NoteDirection.Right
                : NoteDirection.Left;
        }

        return dir.Y > 0
            ? NoteDirection.Down
            : NoteDirection.Up;
    }

    private void EndHold()
    {
        _abilityActive = false;

        _holdCooldownTimer = holdCooldown;

        _requiresGroundReset = true;

    }

    private void HandleTap()
    {
        if (!_input.attack2Pressed)
            return;

        if (_tapCooldownTimer > 0)
            return;

        if (_chimelingIndex <= 0)
            return;

        FireSoundwave();

        _tapCooldownTimer = tapCooldown;
    }

    private void FireSoundwave()
    {
        for(int i = 0, _chimelingIndex, i++) 
        {
            chimeling.EmitSoundwave();

            chimeling.BeginReturn(entity.GlobalPosition);
        }
        _chimelingIndex = 0
    }

    private void RecallChimelings()
    {
        for(int i = 0, _chimelingIndex, i++) 
        {
            chimeling.BeginReturn(entity.GlobalPosition);
        }
        _chimelingIndex = 0
    }

}
