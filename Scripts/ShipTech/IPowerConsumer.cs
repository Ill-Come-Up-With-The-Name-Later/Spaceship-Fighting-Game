using Godot;

namespace SpaceshipFightingGame.Scripts.ShipTech;

public interface IPowerConsumer
{
    [Export] 
    public PowerConsumptionData PowerConsumption { get; set; }
    
    public bool ConsumingPower { get; set; }

    public double CurrentConsumption { get; set; }

    public bool ConsumePowerIfAvailable(Ship ship, double delta);
}