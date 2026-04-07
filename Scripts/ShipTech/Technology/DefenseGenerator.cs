using Godot;
using SpaceshipFightingGame.Scripts.Resources;
using SpaceshipFightingGame.Scripts.Resources.Technology;
using SpaceshipFightingGame.Scripts.ShipTech;

public abstract partial class DefenseGenerator : ShipPart, IRegenerator, IPowerConsumer
{
    [Export]
    public DefenseGeneratorData DefenseGeneratorStats;

    [Export]
    public RegenerationData RegenerationRate { get; set; }

    [Export]
    public PowerConsumptionData PowerConsumption { get; set; }

    public double CurrentConsumption { get; set; }

    public bool ConsumingPower { get; set; }

    public bool DefenseActive { get; set; }

    public bool Regenerating { get; set; }

    public double CurrentDefenseHealth;

    public override void _Ready()
    {
        base._Ready();
        
        CurrentDefenseHealth = DefenseGeneratorStats.DefenseHealth;
        CurrentConsumption = PowerConsumption.Amount;
        
        DefenseActive = false;
        Regenerating = true;
        ConsumingPower = false;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if(Regenerating)
            Regenerate(delta);
    }

    public abstract bool Regenerate(double delta);
    
    public bool ConsumePowerIfAvailable(Ship ship, double delta)
    {
        if (CurrentConsumption > ship.FusionReactor.CurrentPower)
            return false;
        
        ship.FusionReactor.CurrentPower -= CurrentConsumption * delta;

        return true;
    }
}