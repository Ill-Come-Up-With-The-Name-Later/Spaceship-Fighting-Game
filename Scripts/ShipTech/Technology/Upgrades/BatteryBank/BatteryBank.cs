using Godot;

public partial class BatteryBank : Upgrade
{
    [Export]
    public double PowerBonus;
    
    public override void Apply(Ship ship)
    {
        base.Apply(ship);

        ship.FusionReactor.PowerCapacity += PowerBonus;
    }
}