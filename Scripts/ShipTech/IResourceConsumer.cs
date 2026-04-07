using Godot;
using SpaceshipFightingGame.Scripts.Resources;

namespace SpaceshipFightingGame.Scripts.ShipTech;

/// <summary>
/// A part that consumes a resource other than power
/// </summary>
public interface IResourceConsumer
{
    /// <summary>
    /// The rate of resource consumption per second
    /// </summary>
    [Export]
    public ConsumptionData ConsumptionRate { get; set; }
    
    /// <summary>
    /// If the part is consuming resources
    /// </summary>
    public bool Consuming { get; set; }
    
    /// <summary>
    /// Consumes a resource
    /// </summary>
    /// <param name="delta">
    /// Delta time
    /// </param>
    /// <returns>
    /// If resource consumption occured
    /// </returns>
    public bool Consume(double delta);
}