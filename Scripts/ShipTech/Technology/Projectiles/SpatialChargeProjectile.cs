using Godot;
using Godot.Collections;

public partial class SpatialChargeProjectile : Projectile
{
    [Export]
    public double TargetingRadius;
    
    public Ship Target;

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
        Target = FindTarget();
        
        if (Target is not null)
        {
            Vector2 direction = Target.GlobalPosition - GlobalPosition;

            Vector2 velocity = direction.Normalized() * (float)ProjectileStats.Speed;
            Velocity = velocity;
        }
        else
        {
            Velocity *= 0.95f;
            
            if(Velocity.Length() < 1)
                Velocity = Vector2.Zero;
        }
        
        base.Travel(delta);
    }

    public Ship FindTarget()
    {
        Array<Node> nodes = GetTree().GetNodesInGroup("Ship");

        foreach (Node node in nodes)
        {
            if (node is Ship ship)
            {
                if (ship.GlobalPosition.DistanceTo(GlobalPosition) <= TargetingRadius)
                {
                    if (ship != ParentShip && ParentShip.CanTarget(ship))
                        return ship;
                }
            }
        }
        
        return null;
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
        GD.Print("Spatial charge exploded.");
        
        GpuParticles2D explosion = ExplosionParticles.Instantiate<GpuParticles2D>();
        GetTree().CurrentScene.AddChild(explosion);

        explosion.GlobalPosition = GlobalPosition;
        explosion.Restart();
        explosion.Emitting = true;
        
        QueueFree();
    }
}