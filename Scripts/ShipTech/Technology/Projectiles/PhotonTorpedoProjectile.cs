using Godot;

public partial class PhotonTorpedoProjectile : Projectile
{
    public Ship Target;
    public Vector2 fallbackPosition;

    [Export]
    public Color Color;
    
    [Export]
    public PackedScene ExplosionParticles;

    public override void _Ready()
    { 
        GetNode<Sprite2D>("Sprite").Modulate = Color;
    }
    
    public override void Travel(double delta)
    {
        Vector2 targetPosition;
        
        if (IsInstanceValid(Target) && !Target.Cloaked())
        {
            targetPosition = Target.GlobalPosition;
        }
        else
        {
            targetPosition = fallbackPosition;
        }
        
        Vector2 direction = targetPosition - GlobalPosition;

        Vector2 velocity = direction.Normalized() * (float)ProjectileStats.Speed;
        Velocity = velocity;

        if (Target is null && GlobalPosition.DistanceTo(targetPosition) <= 20)
            Explode();
        
        base.Travel(delta);
    }

    public override void OnHit(Area2D area)
    {
        if (area is Hurtbox hurtbox)
        {
            if (hurtbox.ParentShip == ParentShip)
                return;
            
            Explode();
        }
    }

    public override void OnLifespanEnded()
    {
        Explode();
    }

    public void Explode()
    {
        GD.Print("Torpedo exploded.");
        
        GpuParticles2D explosion = ExplosionParticles.Instantiate<GpuParticles2D>();
        GetTree().CurrentScene.AddChild(explosion);

        explosion.GlobalPosition = GlobalPosition;
        explosion.Restart();
        explosion.Emitting = true;
        
        QueueFree();
    }
}