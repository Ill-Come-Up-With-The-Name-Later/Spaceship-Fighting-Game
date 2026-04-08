using Godot;

namespace SpaceshipFightingGame.Scripts.ShipTech;

/// <summary>
/// A part that is a special node on a ship. Ship parts
/// will be direct children of the ship itself.
/// </summary>
public partial class ShipPart : Technology
{
    /// <summary>
    /// Generic data of all ship parts
    /// </summary>
    [Export]
    public GenericShipPartData GenericStats;

    public Ship ParentShip;

    /// <summary>
    /// The health of the part itself
    /// </summary>
    public double CurrentHealth;

    public override void _Ready()
    {
        ParentShip = GetParent() as Ship;
        CurrentHealth = GenericStats.Health;

        if (ParentShip is null)
            GD.PrintErr("Parent ship is null");
    }

    /// <summary>
    /// The percentage of health this part has
    /// </summary>
    /// <returns>
    /// The percentage of health this part has
    /// </returns>
    public double GetHealthPercentage()
    {
        return CurrentHealth / GenericStats.Health;
    }

    /// <summary>
    /// The percentage of health missing on this part
    /// </summary>
    /// <returns>
    /// The percentage of health missing on this part
    /// </returns>
    public double GetMissingHealthPercentage()
    {
        return 1 - CurrentHealth / GenericStats.Health;
    }
}