using Godot;
using SpaceshipFightingGame.Scripts.Resources.Weapons;
using SpaceshipFightingGame.Scripts.ShipTech;

/// <summary>
/// A ship's weapon
/// </summary>
public partial class Weapon : Technology, IPowerConsumer
{
    /// <summary>
    /// The stats of this weapon
    /// </summary>
    [Export]
    public WeaponData WeaponStats { get; set; }
    
    /// <summary>
    /// For weapons, the power consumption
    /// is not per second, it is per shot
    /// </summary>
    [Export]
    public PowerConsumptionData PowerConsumption { get; set; }
    
    /// <summary>
    /// Optional, use if the weapon shoots
    /// a projectile
    /// </summary>
    [Export]
    public Marker2D ProjectileSource;
    
    /// <summary>
    /// Optional, use with ProjectileSource
    /// if the weapon shoots a projectile
    /// </summary>
    [Export]
    public PackedScene ProjectileScene;
    
    public bool ConsumingPower { get; set; }

    public double CurrentConsumption { get; set; }

    public Ship ParentShip;
    
    public Timer FiringCooldownTimer;

    public bool CanFire;
    
    public Eventbus Eventbus;
    
    public override void _Ready()
    {
        CurrentConsumption = PowerConsumption.Amount;
        CanFire = true;
        
        ParentShip = GetParent().GetParent() as Ship;
        
        if(ParentShip is null)
            GD.PrintErr("No owner");
        
        FiringCooldownTimer = GetNode<Timer>("FiringTimer");
        FiringCooldownTimer.WaitTime = WeaponStats.FiringCooldown;
        
        if(FiringCooldownTimer is null)
            GD.PrintErr("No firing cooldown timer");
        
        if(ProjectileScene is not null && ProjectileSource is null)
            GD.PrintErr("No projectile source");
        
        Eventbus = GetNode<Eventbus>("/root/Eventbus");
        
        if(Eventbus is null)
            GD.PrintErr("No eventbus");
        
        FiringCooldownTimer.Timeout += EndFiringCooldown;
    }

    public override void _ExitTree()
    {
        FiringCooldownTimer.Timeout -= EndFiringCooldown;
    }

    /// <summary>
    /// Sets the weapon able to fire again
    /// </summary>
    public void EndFiringCooldown()
    {
        CanFire = true;
    }

    public bool ConsumePowerIfAvailable(Ship ship, double delta)
    {
        if (CurrentConsumption > ship.FusionReactor.CurrentPower)
            return false;

        ship.FusionReactor.CurrentPower -= CurrentConsumption;
        return true;
    }

    /// <summary>
    /// Fires the weapon.
    ///
    /// This should be overriden in children of Weapon
    ///
    /// base.Fire will need to be called at the start of overrides
    /// </summary>
    /// <param name="targetPosition">
    /// The position to fire towards
    /// </param>
    public virtual void Fire(Vector2 targetPosition)
    {
        if (!CanFireWeapon())
            return;
        
        if (!ConsumePowerIfAvailable(ParentShip, 0))
            return;
        
        if (ParentShip.Cloaked())
        {
            Eventbus.EmitSignal("WeaponFired", ParentShip.GlobalPosition, ParentShip, this);
            return;
        }
        
        CanFire = false;
        FiringCooldownTimer.Start();
        
        GD.Print("Weapon fired");
        Eventbus.EmitSignal("WeaponFired", ParentShip.GlobalPosition, ParentShip, this);
    }

    public virtual Projectile FireProjectile(Vector2 targetPosition)
    {
        if (!CanFireWeapon())
            return null;
        
        if (!ConsumePowerIfAvailable(ParentShip, 0))
            return null;
        
        if (ProjectileSource is null)
            return null;

        if (ProjectileSource is null)
            return null;

        if (ParentShip.Cloaked())
        {
            Eventbus.EmitSignal("WeaponFired", ParentShip.GlobalPosition, ParentShip, this);
            return null;
        }
        
        CanFire = false;
        FiringCooldownTimer.Start();

        Projectile proj = (Projectile)ProjectileScene.Instantiate();
        SetupProjectile(proj, targetPosition);
        Eventbus.EmitSignal("WeaponFired", ParentShip.GlobalPosition, ParentShip, this);
        GD.Print("Fired projectile");

        return proj;
    }

    /// <summary>
    /// Fires the weapon
    ///
    /// This should be overriden in children of Weapon
    ///
    /// base.Fire will need to be called at the start of overrides
    /// </summary>
    /// <param name="target">
    /// Another ship to target
    /// </param>
    public virtual void Fire(Ship target)
    {
        Fire(target.GlobalPosition);
    }

    /// <summary>
    /// If the weapon can fire
    /// </summary>
    /// <returns>
    /// If the weapon can fire
    /// </returns>
    public virtual bool CanFireWeapon()
    {
        return CanFire;
    }
    
    /// <summary>
    /// Sets up a projectile's basic properties.
    /// Override this if you need a homing projectile
    /// and you have to set the target.
    /// </summary>
    /// <param name="projectile">
    /// The projectile
    /// </param>
    public virtual void SetupProjectile(Projectile projectile, Vector2 targetPosition)
    {
        GetParent().GetTree().Root.AddChild(projectile);
        
        projectile.ParentWeapon = this;
        projectile.ParentShip = ParentShip;
        projectile.ParentWeaponStats = WeaponStats;
        projectile.GlobalPosition = ProjectileSource.GlobalPosition 
                                    + (targetPosition - projectile.GlobalPosition).Normalized() * 25;
        
        projectile.LookAt(targetPosition);
        projectile.Velocity = (targetPosition - projectile.GlobalPosition).Normalized()
                        * (float)projectile.ProjectileStats.Speed;
    }
}