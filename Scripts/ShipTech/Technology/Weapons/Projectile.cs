using Godot;
using SpaceshipFightingGame.Scripts.Resources.Weapons;

/// <summary>
/// A projectile shot by a weapon
/// </summary>
public partial class Projectile : Node2D
{
    public WeaponData ParentWeaponStats;
    
    [Export]
    public ProjectileData ProjectileStats;
    
    public Weapon ParentWeapon;

    public Timer LifespanTimer;

    public override void _Ready()
    {
        ParentWeapon = GetParent() as Weapon;
        
        if(ParentWeapon is null)
            GD.PrintErr("No parent weapon found");
 
        ParentWeaponStats = ParentWeapon.WeaponStats;
        
        if(ParentWeaponStats is null)
            GD.PrintErr("Parent weapon stats not found");
        
        LifespanTimer = GetNode<Timer>("LifespanTimer");
    }
}