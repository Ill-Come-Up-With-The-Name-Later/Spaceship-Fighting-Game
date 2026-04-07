using Godot;
using SpaceshipFightingGame.Scripts.Resources;

namespace SpaceshipFightingGame.Scripts.ShipTech;

/// <summary>
/// A part that regenerates a resource of any type
/// </summary>
public interface IRegenerator
{
    /// <summary>
    /// The rate of regeneration per second
    /// </summary>
    [Export]
    public RegenerationData RegenerationRate { get; set; }
    
    /// <summary>
    /// If the part is regenerating
    /// </summary>
    public bool Regenerating { get; set; }
    
    /// <summary>
    /// Regenerates some type of resource
    /// </summary>
    /// <param name="delta">
    /// Delta time
    /// </param>
    /// <returns>
    /// If any regeneration occured
    /// </returns>
    public bool Regenerate(double delta);
}