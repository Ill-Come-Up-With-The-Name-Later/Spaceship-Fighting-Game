using Godot;
using SpaceshipFightingGame.Scripts.Resources;

namespace SpaceshipFightingGame.Scripts.ShipTech;

public interface IConsumer
{
    [Export]
    public ConsumptionData ConsumptionRate { get; set; }
    
    public bool Active { get; set; }
    
    public bool Consume(double delta);
}