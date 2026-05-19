using Godot;

public partial class PlayableCharacterData : Node
{
    public float JumpForce { get; private set; }

    public float JumpHeight
    {
        get;
        set
        {
            field = value;
            UpdateGravity();
        }
    }
    
    public float JumpTimeToApex
    {
        get;
        set
        {
            field = value;
            UpdateGravity();
        }
    }

    public float Gravity {
        get;
        private set
        {
            GD.Print(value);
            field = value;
            JumpForce = value * JumpTimeToApex;
        }
    }

    private void UpdateGravity()
    {
        if (JumpTimeToApex == 0)
        {
            return;
        }

        Gravity = 2f * JumpHeight / (JumpTimeToApex * JumpTimeToApex);
    }
}