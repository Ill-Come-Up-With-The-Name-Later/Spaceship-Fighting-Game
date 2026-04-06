using Godot;

namespace SpaceshipFightingGame.Scripts.ShipTech;

public interface IPowerConsumer
{
    [Export] 
    public PowerConsumptionData PowerConsumption { get; set; }
    
    public bool Consuming { get; set; }

    public double CurrentConsumption { get; set; }

    public virtual void ConsumePowerIfAvailable(double currentPower, double delta)
    {
        if (CurrentConsumption > currentPower)
        {
            return;
        }
    }
}