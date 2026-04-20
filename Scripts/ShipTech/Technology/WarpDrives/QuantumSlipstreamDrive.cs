using Godot;
using Godot.Collections;
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

    [Export]
    public double ShieldDamagingRadius;

    public bool Consuming { get; set; }

    public bool Regenerating { get; set; }

    public Eventbus Eventbus;
    
    public override void _Ready()
    {
        base._Ready();
        Eventbus = GetNode<Eventbus>("/root/Eventbus");
        SlipstreamPower = Stats.SlipstreamPower;
    }

    public override bool CanActivate()
    {
        return ParentShip.CurrentPower >=
               CurrentConsumption * 1.5 && CurrentHealth > 0 &&
               ParentShip.Hull.GetHealthPercentage() >= 0.6
               && ParentShip.Engine.CurrentHealth > 0;
    }

    public override void ActivateWarp()
    {
        base.ActivateWarp();
        Consuming = true;
        Regenerating = false;
        
        if(ParentShip.HasPart<ShieldGenerator>() && ParentShip.DefenseSystem.DefenseActive)
            ParentShip.DefenseSystem.Toggle();
    }

    public override void ExitWarpState()
    {
        base.ExitWarpState();
        Consuming = false;
        Regenerating = true;

        DamageNearbyShields();
    }

    public void DamageNearbyShields()
    {
        Array<Node> nodes = GetTree().GetNodesInGroup("Ship");

        foreach (Node node in nodes)
        {
            if (node is Ship ship)
            {
                if (ship.GlobalPosition.DistanceTo(GlobalPosition) <= ShieldDamagingRadius)
                {
                    if (ship != ParentShip)
                    {
                        Eventbus.EmitSignal("ShipDamaged", ship, ParentShip, 0, 
                            ship.DefenseSystem.DefenseGeneratorStats.DefenseHealth * (1 - Stats.HealthDrain), 0);
                        
                        if(ship.HasShield())
                            ship.DefenseSystem.Toggle();
                    }
                }
            }
        }
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

        if (Active && WarpDriveState is WarpDriveState.Warping)
        {
            Eventbus.EmitSignal("ShipDamaged", ParentShip, ParentShip, 0, 0,
                ParentShip.Hull.GenericStats.Health * 0.1 * delta);
        }

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