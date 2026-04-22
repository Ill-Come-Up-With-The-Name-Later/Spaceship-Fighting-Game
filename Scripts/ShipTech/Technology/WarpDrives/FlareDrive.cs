using Godot;
using Godot.Collections;

public partial class FlareDrive : WarpDrive
{
    [Export]
    public FlareDriveStats Stats;
    
    public override bool CanActivate()
    {
        return ParentShip.CurrentPower >= 
               CurrentConsumption * 1.2 && CurrentHealth > 0 &&
               ParentShip.Engine.GetHealthPercentage() >= 0.4;
    }
    
    public override void EnterAcceleration()
    {
        base.EnterAcceleration();
        ParentShip.DamageEngine(ParentShip.Engine.GenericStats.Health * 0.25);
        DamageNearbyShips();
    }
    
    public void DamageNearbyShips()
    {
        Array<Node> nodes = GetTree().GetNodesInGroup("Ship");

        foreach (Node node in nodes)
        {
            if (node is Ship ship)
            {
                if (ship.GlobalPosition.DistanceTo(GlobalPosition) <= Stats.DamagingRadius)
                {
                    if (ship != ParentShip)
                    {
                        Eventbus.EmitSignal("ShipDamaged", ship, ParentShip, Stats.Damage, 0, 0);
                        
                        if(ship.HasShield())
                            ship.DefenseSystem.Toggle();
                    }
                }
            }
        }
    }
}
