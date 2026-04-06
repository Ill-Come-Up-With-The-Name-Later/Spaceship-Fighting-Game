using Godot;
using SpaceshipFightingGame.Scripts.Resources;

namespace SpaceshipFightingGame.Scripts.ShipTech;

public interface IRegenerator
{
    [Export]
    public RegenerationData RegenerationRate { get; set; }
    
    public bool Regenerating { get; set; }
    
    public bool Regenerate(double delta);
}