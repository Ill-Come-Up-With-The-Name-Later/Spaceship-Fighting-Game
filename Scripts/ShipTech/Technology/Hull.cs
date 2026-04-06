using Godot;
using SpaceshipFightingGame.Scripts.Resources.Technology;
using SpaceshipFightingGame.Scripts.ShipTech;

public partial class Hull : ShipPart
{
    [Export]
    public HullData HullStats;
    
    public bool Active { get; set; }

    public bool Regenerating { get; set; }
    
    public double CurrentMaterials;
    public double CurrentScraps;

    public override void _Ready()
    {
        base._Ready();
        
        CurrentMaterials = HullStats.Materials;
        CurrentScraps = HullStats.Scrap;
    }
}