using Godot;

public partial class BatteryBank : Upgrade
{
    public override void Apply(Ship ship)
    {
        base.Apply(ship);

        ship.FusionReactor.PowerCapacity *= 2;
    }
}