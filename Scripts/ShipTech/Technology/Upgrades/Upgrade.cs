using System.Collections.Generic;
using SpaceshipFightingGame.Scripts.ShipTech;

public partial class Upgrade : Technology
{
    public HashSet<Ship> Applied;

    public override void _Ready()
    {
        Applied = new HashSet<Ship>();
    }
    
    /// <summary>
    /// Applies the upgrade. For most upgrades,
    /// overriding this should be sufficient
    /// </summary>
    /// <param name="ship">
    /// The ship to apply the upgrade to
    /// </param>
    public virtual void Apply(Ship ship)
    {
        if (Applied.Contains(ship))
            return;
        
        Applied.Add(ship);
    }
}