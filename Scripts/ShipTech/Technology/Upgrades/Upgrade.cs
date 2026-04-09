using System.Collections.Generic;
using Godot;

public partial class Upgrade : Node2D
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