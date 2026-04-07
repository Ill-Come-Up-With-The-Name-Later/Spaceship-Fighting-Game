using Godot;

namespace SpaceshipFightingGame.Scripts.ShipTech;

public partial class ShipPart : Technology
{
    [Export]
    public GenericShipPartData GenericStats;

    public Ship ParentShip;

    public double CurrentHealth;

    public override void _Ready()
    {
        ParentShip = GetParent() as Ship;
        CurrentHealth = GenericStats.Health;

        if (ParentShip is null)
            GD.PrintErr("Parent ship is null");
    }

    public double GetHealthPercentage()
    {
        return CurrentHealth / GenericStats.Health;
    }

    public double GetMissingHealthPercentage()
    {
        return 1 - CurrentHealth / GenericStats.Health;
    }
}