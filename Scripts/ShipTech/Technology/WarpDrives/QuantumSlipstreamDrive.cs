using Godot;
using SpaceshipFightingGame.Scripts.Resources;
using SpaceshipFightingGame.Scripts.Resources.Technology;
using SpaceshipFightingGame.Scripts.ShipTech;

public partial class QuantumSlipstreamDrive : WarpDrive, IResourceConsumer, IRegenerator
{
    [Export] 
    public QuantumSlipstreamDriveStats Stats;

    public double SlipstreamPower;

    [Export]
    public RegenerationData RegenerationRate { get; set; }

    [Export]
    public ConsumptionData ConsumptionRate { get; set; }

    public bool Consuming { get; set; }

    public bool Regenerating { get; set; }
    
    public override void _Ready()
    {
        base._Ready();
        SlipstreamPower = Stats.SlipstreamPower;
    }

    public override bool CanActivate()
    {
        return ParentShip.CurrentPower >=
               CurrentConsumption * 1.5 && CurrentHealth > 0 &&
               ParentShip.Hull.GetHealthPercentage() >= 0.6;
    }

    public override void ActivateWarp()
    {
        base.ActivateWarp();
        Consuming = true;
        Regenerating = false;
    }

    public override void ExitWarpState()
    {
        base.ExitWarpState();
        // Damage and deactivate nearby enemies' shields
        Consuming = false;
        Regenerating = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        if(!Active)
            if(Regenerating)
                Regenerate(delta);
        
        if(Active)
            if(Consuming)
                if (!Consume(delta))
                {
                    DeactivateWarp();
                    GD.Print("Out of slipstream power");
                }

        if(Active && WarpDriveState is WarpDriveState.Warping)
            ParentShip.DamageHull(ParentShip.Hull.GenericStats.Health * 0.1 * delta);

        if (ParentShip.Hull.GetHealthPercentage() < 0.6 && Active && 
            WarpDriveState is WarpDriveState.Warping or WarpDriveState.Accelerating)
        {
            DeactivateWarp();
            GD.Print("Health low");
        }
    }

    public bool Consume(double delta)
    {
        if (SlipstreamPower < SlipstreamPower - ConsumptionRate.PerSecondRate * delta)
            return false;
        
        SlipstreamPower = Mathf.Max(0, SlipstreamPower - ConsumptionRate.PerSecondRate * delta);
        return true;
    }

    public bool Regenerate(double delta)
    {
        if (SlipstreamPower < Stats.SlipstreamPower)
        {
            SlipstreamPower = Mathf.Min(Stats.SlipstreamPower,
                SlipstreamPower + RegenerationRate.PerSecondRate * delta);
            return true;
        }

        return false;
    }
}