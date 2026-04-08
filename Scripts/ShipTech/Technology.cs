using Godot;
using SpaceshipFightingGame.Scripts.Resources;

namespace SpaceshipFightingGame.Scripts.ShipTech;

/// <summary>
/// Any sort of technology a ship can have
/// </summary>
public partial class Technology : Node2D
{
    [Export]
    public TechWeightData Weights;
    
    [Export]
    public double Level = 1;
}