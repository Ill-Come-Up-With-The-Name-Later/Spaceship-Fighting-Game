using Godot;
using SpaceshipFightingGame.Scripts.Resources;
using SpaceshipFightingGame.Scripts.ShipTech;

/// <summary>
/// Attached as a child of ship parts, used to allow
/// them to regenerate health by consuming materials
/// </summary>
public partial class ShipPartRegenerator : Node2D, IRegenerator, IResourceConsumer, IToggleable
{
    [Export]
    public RegenerationData RegenerationRate { get; set; }
    
    [Export]
    public ConsumptionData ConsumptionRate { get; set; }
    
    public bool Regenerating { get; set; }

    public bool Consuming { get; set; }

    public bool CanToggle { get; set; }

    public ShipPart ParentPart;
    
    public override void _Ready()
    {
        base._Ready();
        ParentPart = GetParent<ShipPart>();

        if (ParentPart is null)
            GD.PrintErr("Part regenerator has no target.");
        
        Regenerating = true;
        Consuming = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (ParentPart.ParentShip.Materials < ConsumptionRate.PerSecondRate * delta)
            CanToggle = false;
        else
            CanToggle = true;

        if (ParentPart.CurrentHealth >= ParentPart.GenericStats.Health)
            Consuming = false;

        if (ParentPart.CurrentHealth < ParentPart.GenericStats.Health && Regenerating)
            Consuming = true;
        
        if(Regenerating && Consuming)
            if(Consume(delta))
                Regenerate(delta);
    }

    public bool Consume(double delta)
    {
        if (ParentPart.ParentShip.Materials < ConsumptionRate.PerSecondRate * delta)
            return false;
        
        ParentPart.ParentShip.Materials = Mathf.Max(0,
            ParentPart.ParentShip.Materials - ConsumptionRate.PerSecondRate * delta);
        return true;
    }

    public bool Regenerate(double delta)
    {
        if (ParentPart.CurrentHealth >= ParentPart.GenericStats.Health)
            return false;
        
        ParentPart.CurrentHealth = Mathf.Min(ParentPart.GenericStats.Health, 
            ParentPart.CurrentHealth + RegenerationRate.PerSecondRate * delta);
        return true;
    }

    public void Toggle()
    {
        if (!CanToggle)
            return;
        
        Regenerating = !Regenerating;
        Consuming = Regenerating;
    }
}