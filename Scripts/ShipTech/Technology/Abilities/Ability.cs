using Godot;
using SpaceshipFightingGame.Scripts.ShipTech;

/// <summary>
/// An activatable ability
/// </summary>
public abstract partial class Ability : Technology
{
    [Export]
    public double Cooldown;
    
    public Ship ParentShip;
    public Timer CooldownTimer;
    public bool CanUse;
    
    public Eventbus Eventbus;

    public override void _Ready()
    {
        ParentShip = GetParent().GetParent() as Ship;

        if (ParentShip is null)
            GD.PrintErr("No parent ship found");

        CooldownTimer = GetNode<Timer>("CooldownTimer");

        if (CooldownTimer is null)
            GD.PrintErr("No cooldown timer");

        CooldownTimer.WaitTime = Cooldown;
        CooldownTimer.Timeout += ReEnable;
        
        Eventbus = GetNode<Eventbus>("/root/Eventbus");
        
        if (Eventbus is null)
            GD.PrintErr("No eventbus found");
        
        CanUse = true;
    }

    public override void _ExitTree()
    {
        CooldownTimer.Timeout -= ReEnable;
    }

    public void Activate()
    {
        if (!CanUse)
            return;
        
        CanUse = false;
        CooldownTimer.Start();
        Eventbus.EmitSignal("AbilityUsed", this, ParentShip);
        
        GD.Print("Ability activated");
        Use();
    }

    /// <summary>
    /// What happens when the ability is used.
    /// This is the override to specify what happens
    /// when the ability activates.
    /// </summary>
    public abstract void Use();

    public void ReEnable()
    {
        CanUse = true;
    }
}