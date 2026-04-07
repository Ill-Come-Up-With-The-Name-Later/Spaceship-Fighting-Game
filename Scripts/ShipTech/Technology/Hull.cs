using Godot;
using SpaceshipFightingGame.Scripts.Resources.Technology;
using SpaceshipFightingGame.Scripts.ShipTech;

public partial class Hull : ShipPart
{
    /// <summary>
    /// The hull's health and how many
    /// resources are stored
    /// </summary>
    [Export]
    public HullData HullStats;
    
    public double CurrentMaterials;
    public double CurrentScraps;

    public override void _Ready()
    {
        base._Ready();
        
        CurrentMaterials = HullStats.Materials;
        CurrentScraps = HullStats.Scrap;
    }
}