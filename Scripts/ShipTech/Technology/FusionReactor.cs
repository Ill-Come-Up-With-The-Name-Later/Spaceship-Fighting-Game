using Godot;
using SpaceshipFightingGame.Scripts.Resources;
using SpaceshipFightingGame.Scripts.Resources.Technology;
using SpaceshipFightingGame.Scripts.ShipTech;

public partial class FusionReactor : ShipPart, IRegenerator, IResourceConsumer, IToggleable
{
    /// <summary>
    /// The reactor's fuel and power capacity
    /// </summary>
    [Export]
    public FusionReactorData ReactorStats;

    [Export]
    public RegenerationData RegenerationRate { get; set; }
    
    [Export]
    public ConsumptionData ConsumptionRate { get; set; }
    
    public double CurrentPower;
    public double CurrentFuel;

    public bool Consuming { get; set; }
    public bool Regenerating { get; set; }

    public bool CanToggle { get; set; }

    public override void _Ready()
    {
        base._Ready();
        
        CurrentPower = ReactorStats.PowerCapacity;
        CurrentFuel = ReactorStats.Fuel;
        
        Consuming = true;
        Regenerating = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (CurrentFuel == 0)
        {
            Regenerating = false;
            Consuming = false;
            CanToggle = false;
            return;
        }
        
        Consuming = Regenerating;
        CanToggle = true;

        if(Regenerating)
            if(Consuming && Regenerate(delta))
                Consume(delta);
    }
    
    public void Toggle()
    {
        if (!CanToggle)
            return;
        
        Regenerating = !Regenerating;
    }

    public bool Regenerate(double delta)
    {
        if (CurrentPower < ReactorStats.PowerCapacity)
        {
            CurrentPower = Mathf.Max(ReactorStats.PowerCapacity,
                CurrentPower + (RegenerationRate.PerSecondRate * delta));
            
            return true;
        }
        
        return false;
    }

    public bool Consume(double delta)
    {
        if (CurrentFuel >= ConsumptionRate.PerSecondRate * delta)
        {
            CurrentFuel = Mathf.Max(0, CurrentFuel - (ConsumptionRate.PerSecondRate * delta));
            return true;
        }
        
        return false;
    }
}