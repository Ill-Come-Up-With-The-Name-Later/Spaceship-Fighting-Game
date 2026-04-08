using Godot;

namespace SpaceshipFightingGame.Scripts.Resources.Weapons;

/// <summary>
/// General stats of weapons
/// </summary>
public partial class WeaponData : Resource
{
    /// <summary>
    /// Standard damage
    /// </summary>
    [Export]
    public double Damage;

    /// <summary>
    /// Only damages shields/armor
    /// </summary>
    [Export]
    public double ShieldDamage;
    
    /// <summary>
    /// Penetrates shields and armor but
    /// is half as effective if they're active
    /// </summary>
    [Export]
    public double PenetratingDamage;

    /// <summary>
    /// The delay between firing the weapon
    /// </summary>
    [Export] 
    public double FiringCooldown;
}