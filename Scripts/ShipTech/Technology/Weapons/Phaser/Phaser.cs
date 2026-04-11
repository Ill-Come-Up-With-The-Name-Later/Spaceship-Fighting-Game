using Godot;
using Godot.Collections;

public partial class Phaser : Weapon
{
    [Export]
    public double HitEffectRadius;

    [Export] 
    public Color Color;
    
    public Line2D Line;
    public RayCast2D Ray;

    public Timer EffectTimer;

    public override void _Ready()
    {
        base._Ready();
        
        Line = GetNode<Line2D>("PhaserBeam");
        
        if(Line is null)
            GD.PrintErr("Phaser Line is null");
        
        Line.DefaultColor = Color;
        Line.Visible = false;
        
        Ray = GetNode<RayCast2D>("RayCast");
        
        if(Ray is null)
            GD.PrintErr("Ray cast is null");
        
        EffectTimer = GetNode<Timer>("EffectTimer");
        EffectTimer.Timeout += HideLine;
    }
    
    public override void Fire(Vector2 targetPosition)
    {
        base.Fire(targetPosition);
        
        Line.Points = new Vector2[] { Line.ToLocal(targetPosition - ParentShip.GlobalPosition).Normalized()
                                                   * ParentShip.Sprite.Texture.GetHeight() / 2, Line.ToLocal(targetPosition) };
        Line.Visible = true;
        
        EffectTimer.Start();
        HitTargetsNear(targetPosition);
    }

    public void HitTargetsNear(Vector2 targetPosition)
    {
        Array<Node> nodes = GetTree().GetNodesInGroup("Ship");

        foreach (Node node in nodes)
        {
            if (node is Ship ship)
            {
                if (ship.GlobalPosition.DistanceTo(targetPosition) <= HitEffectRadius)
                {
                    if (ship != ParentShip)
                        ship.Hurtbox.ApplyDamage(ship, ParentShip, WeaponStats.Damage,
                            WeaponStats.ShieldDamage, WeaponStats.PenetratingDamage);
                }
            }
        }
    }

    public void HideLine()
    {
        Line.Visible = false;
    }
}