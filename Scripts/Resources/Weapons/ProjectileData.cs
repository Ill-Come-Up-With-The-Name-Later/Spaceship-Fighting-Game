using Godot;

namespace SpaceshipFightingGame.Scripts.Resources.Weapons;

/// <summary>
/// Projectile stats
/// </summary>
public partial class ProjectileData : Resource
{
    /// <summary>
    /// The projectile's lifespan in seconds
    /// </summary>
    [Export] 
    public double Lifespan;

    /// <summary>
    /// The projectile's speed
    /// </summary>
    [Export]
    public double Speed;
}