using Godot;
using SpaceshipFightingGame.Scripts.Resources;
using SpaceshipFightingGame.Scripts.Resources.Technology;
using SpaceshipFightingGame.Scripts.ShipTech;

public abstract partial class DefenseGenerator : ShipPart, IRegenerator, IPowerConsumer, IToggleable
{
    /// <summary>
    /// The amount of health the defense has and how much
    /// damage reduction it provides
    /// </summary>
    [Export]
    public DefenseGeneratorData DefenseGeneratorStats;

    [Export]
    public RegenerationData RegenerationRate { get; set; }

    [Export]
    public PowerConsumptionData PowerConsumption { get; set; }

    public bool CanToggle { get; set; }

    public double CurrentConsumption { get; set; }

    public bool ConsumingPower { get; set; }
    
    /// <summary>
    /// If the defense generator is active
    /// </summary>
    public bool DefenseActive { get; set; }

    public bool Regenerating { get; set; }

    /// <summary>
    /// The health on the defense system, not the part itself
    /// </summary>
    public double CurrentDefenseHealth;
    
    /// <summary>
    /// The percent of damage reduction being
    /// provided
    /// </summary>
    public double DefensePercentage;

    public override void _Ready()
    {
        base._Ready();
        
        CurrentDefenseHealth = DefenseGeneratorStats.DefenseHealth;
        CurrentConsumption = PowerConsumption.Amount;
        
        DefenseActive = false;
        Regenerating = true;
        ConsumingPower = false;

        DefensePercentage = DefenseGeneratorStats.DamageReductionPercent;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (CurrentHealth <= 0)
        {
            DefenseActive = false;
            CurrentDefenseHealth = 0;
        }
        
        if(CurrentDefenseHealth <= 0)
            DefenseActive = false;
    }

    public void Toggle()
    {
        if (!CanToggle)
            return;
        
        DefenseActive = !DefenseActive;
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