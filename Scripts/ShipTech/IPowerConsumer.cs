using Godot;

namespace SpaceshipFightingGame.Scripts.ShipTech;

/// <summary>
/// A part that consumes power
/// </summary>
public interface IPowerConsumer
{
    /// <summary>
    /// The rate of power consumption per second
    /// </summary>
    [Export] 
    public PowerConsumptionData PowerConsumption { get; set; }
    
    /// <summary>
    /// If the part is consuming power
    /// </summary>
    public bool ConsumingPower { get; set; }

    /// <summary>
    /// The amount of power consumed per second
    /// </summary>
    public double CurrentConsumption { get; set; }

    /// <summary>
    /// Consumes power if it is available
    /// </summary>
    /// <param name="ship">
    /// The ship using power
    /// </param>
    /// <param name="delta">
    /// Delta time
    /// </param>
    /// <returns>
    /// If the ship was able to consume power
    /// </returns>
    public bool ConsumePowerIfAvailable(global::Ship ship, double delta);
}