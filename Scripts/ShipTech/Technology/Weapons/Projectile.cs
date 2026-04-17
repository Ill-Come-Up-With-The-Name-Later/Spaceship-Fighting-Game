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

    public override void _Ready()
    {
        Hitbox = GetNode<Hitbox>("Hitbox");
        
        if(Hitbox is null)
            GD.PrintErr("Hitbox not found");
        
        LifespanTimer = GetNode<Timer>("LifespanTimer");
        
        if(LifespanTimer is null)
            GD.PrintErr("Lifespan timer not found");
        
        LifespanTimer.WaitTime = ProjectileStats.Lifespan;
        
        Sprite = GetNode<Sprite2D>("Sprite");

        Hitbox.AreaEntered += OnHit;
        
        LifespanTimer.Timeout += OnLifespanEnded;
        LifespanTimer.Start();
        
        Sprite.GlobalRotation = Velocity.Angle() - Mathf.Pi / 2;
    }

    public override void _ExitTree()
    {
        if(LifespanTimer is not null)
            LifespanTimer.Timeout -= OnLifespanEnded;
    }

    public override void _PhysicsProcess(double delta)
    {
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
        if (Sprite is not null)
            Sprite.GlobalRotation = Velocity.Angle() - Mathf.Pi / 2;

        MoveAndCollide(Velocity);
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
        if (area is Hurtbox hurtbox)
        {
            if (hurtbox.ParentShip == ParentShip)
                return;
            
            QueueFree();
        }
    }

    /// <summary>
    /// Runs when the projectile's lifespan ends
    /// </summary>
    public virtual void OnLifespanEnded()
    {
    }
}