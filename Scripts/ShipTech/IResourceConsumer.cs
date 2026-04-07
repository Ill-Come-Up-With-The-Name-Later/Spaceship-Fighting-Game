using Godot;
using SpaceshipFightingGame.Scripts.Resources;

namespace SpaceshipFightingGame.Scripts.ShipTech;

public interface IResourceConsumer
{
    [Export]
    public ConsumptionData ConsumptionRate { get; set; }
    
    public bool Consuming { get; set; }
    
    public bool Consume(double delta);
}