using Godot;

namespace SpaceshipFightingGame.Scripts.Resources;

/// <summary>
/// Weights of technology for random
/// selection and for the enemy targeting system.
/// </summary>
public partial class TechWeightData : Resource
{
    /// <summary>
    /// Weight of the technology to the enemy
    /// targeting system
    /// </summary>
    [Export]
    public double StrengthWeight;
    
    /// <summary>
    /// Weight of the technology in the enemy
    /// loadout generation.
    /// </summary>
    [Export]
    public double Weight;
}