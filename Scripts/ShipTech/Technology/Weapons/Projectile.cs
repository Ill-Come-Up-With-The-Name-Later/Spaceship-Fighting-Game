using Godot;
using SpaceshipFightingGame.Scripts.Resources.Weapons;

/// <summary>
/// A projectile shot by a weapon
/// </summary>
public partial class Projectile : CharacterBody2D
{
    public WeaponData ParentWeaponStats;
    
    [Export]
    public ProjectileData ProjectileStats;
    
    public Weapon ParentWeapon;

    public Timer LifespanTimer;
    
    public Hitbox Hitbox;

    public override void _Ready()
    {
        ParentWeapon = GetParent() as Weapon;
        
        if(ParentWeapon is null)
            GD.PrintErr("No parent weapon found");
 
        ParentWeaponStats = ParentWeapon.WeaponStats;
        
        if(ParentWeaponStats is null)
            GD.PrintErr("Parent weapon stats not found");
        
        Hitbox = GetNode<Hitbox>("Hitbox");
        
        if(Hitbox is null)
            GD.PrintErr("Hitbox not found");
        
        LifespanTimer = GetNode<Timer>("LifespanTimer");
        
        if(LifespanTimer is null)
            GD.PrintErr("Lifespan timer not found");

        LifespanTimer.Start();
        LifespanTimer.Timeout += OnLifespanEnded;

        Hitbox.AreaEntered += OnHit;
    }

    public override void _ExitTree()
    {
        LifespanTimer.Timeout -= OnLifespanEnded;
    }

    public override void _PhysicsProcess(double delta)
    {
       Travel(delta);
    }

    /// <summary>
    /// While the projectile is traveling
    /// </summary>
    /// <param name="delta">
    /// Delta time
    /// </param>
    public virtual void Travel(double delta)
    {
        MoveAndSlide();
    }

    /// <summary>
    /// When the projectile hits a ship
    ///
    /// Overrides should call base.OnHit
    /// </summary>
    /// <param name="area">
    /// The other area that has been hit
    /// </param>
    public virtual void OnHit(Area2D area)
    {
        if (area.GetParent() is Hurtbox hurtbox)
            if (hurtbox.ParentShip == ParentWeapon.ParentShip)
                return;
    }

    /// <summary>
    /// Runs when the projectile's lifespan ends
    /// </summary>
    public virtual void OnLifespanEnded()
    {
        QueueFree();
    }
}