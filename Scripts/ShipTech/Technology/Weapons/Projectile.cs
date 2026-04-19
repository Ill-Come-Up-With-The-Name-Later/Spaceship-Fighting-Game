using Godot;
using SpaceshipFightingGame.Scripts.Resources.Weapons;

/// <summary>
/// A projectile shot by a weapon
/// </summary>
public partial class Projectile : CharacterBody2D
{
    public Weapon ParentWeapon;
    public WeaponData ParentWeaponStats;
    
    [Export]
    public ProjectileData ProjectileStats;
    
    public Ship ParentShip;
    public Timer LifespanTimer;
    public Hitbox Hitbox;
    public Sprite2D Sprite;
    public Eventbus Eventbus;

    public override void _Ready()
    {
        Hitbox = GetNode<Hitbox>("Hitbox");
        
        if(Hitbox is null)
            GD.PrintErr("Hitbox not found");
        
        LifespanTimer = GetNode<Timer>("LifespanTimer");
        
        if(LifespanTimer is null)
            GD.PrintErr("Lifespan timer not found");
        
        Eventbus = GetNode<Eventbus>("/root/Eventbus");
        
        if(Eventbus is null)
            GD.PrintErr("Eventbus not found");
        
        Eventbus.SimplifiedShipDamaged += OnHit;
        
        LifespanTimer.WaitTime = ProjectileStats.Lifespan;
        
        Sprite = GetNode<Sprite2D>("Sprite");
        
        LifespanTimer.Timeout += OnLifespanEnded;
        LifespanTimer.Start();
        
        Sprite.GlobalRotation = Velocity.Angle() - Mathf.Pi / 2;
    }

    public override void _ExitTree()
    {
        GD.Print(GetName() + " destroyed");
        
        if(LifespanTimer is not null)
            LifespanTimer.Timeout -= OnLifespanEnded;
        
        if(Eventbus is not null)
            Eventbus.SimplifiedShipDamaged -= OnHit;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsInstanceValid(this))
            return;
        
        Travel(delta);
    }

    /// <summary>
    /// While the projectile is traveling.
    /// Overrides should call base.Travel
    /// </summary>
    /// <param name="delta">
    /// Delta time
    /// </param>
    public virtual void Travel(double delta)
    {
        if (!IsInstanceValid(this))
            return;
        
        if (Sprite is not null)
            Sprite.GlobalRotation = Velocity.Angle() - Mathf.Pi / 2;
        
        MoveAndCollide(Velocity);
    }

    /// <summary>
    /// When the projectile hits a ship
    ///
    /// Overrides should call base.OnHit
    /// </summary>
    /// <param name="damaged">
    /// The damaged ship
    /// </param>
    /// <param name="damager">
    /// The ship dealing damage
    /// </param>
    public virtual void OnHit(Ship damaged, Ship damager)
    {
        if(!IsInstanceValid(this))
            GD.PrintErr("Projectile couldn't find itself");
        
        GD.Print(damager.GetName() + "'s " + GetName() + " hit " + damaged.GetName());
        QueueFree();
    }

    /// <summary>
    /// Runs when the projectile's lifespan ends
    /// </summary>
    public virtual void OnLifespanEnded()
    {
        if (!IsInstanceValid(this))
            return;
        
        QueueFree();
    }
}