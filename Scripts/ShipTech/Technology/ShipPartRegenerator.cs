using Godot;
using SpaceshipFightingGame.Scripts.Resources;
using SpaceshipFightingGame.Scripts.ShipTech;

public partial class ShipPartRegenerator : Node2D, IRegenerator, IResourceConsumer
{
    [Export]
    public RegenerationData RegenerationRate { get; set; }
    
    [Export]
    public ConsumptionData ConsumptionRate { get; set; }
    
    public bool Regenerating { get; set; }

    public bool Consuming { get; set; }

    public ShipPart ParentPart;
    
    public override void _Ready()
    {
        base._Ready();
        ParentPart = GetParent<ShipPart>();
        
        Regenerating = false;
        Consuming = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (ParentPart.CurrentHealth < ParentPart.GenericStats.Health && ParentPart.ParentShip.Materials > 0)
        {
            Regenerating = true;
            Consuming = true;
        }
        else
        {
            Regenerating = false;
            Consuming = false;
        }
        
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
}