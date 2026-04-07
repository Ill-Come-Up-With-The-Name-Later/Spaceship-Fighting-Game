using Godot;

namespace SpaceshipFightingGame.Scripts.ShipTech;

/// <summary>
/// Any sort of technology a ship can have
/// </summary>
public partial class Technology : Node2D
{
    [Export]
    public double Level = 1;
}