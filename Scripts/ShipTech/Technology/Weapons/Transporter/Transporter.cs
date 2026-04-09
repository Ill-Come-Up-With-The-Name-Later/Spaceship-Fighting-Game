using System;
using Godot;
using Godot.Collections;

public partial class Transporter : Weapon
{
    [Export]
    public double HitEffectRadius;

    [Export] 
    public Color Color;
    
    public Line2D Line;
    public RayCast2D Ray;

    public Timer EffectTimer;

    public override bool CanFireWeapon()
    {
        return CanFire && !ParentShip.HasShield();
    }

    public override void _Ready()
    {
        base._Ready();
        
        Line = GetNode<Line2D>("TransporterBeam");
        
        if(Line is null)
            GD.PrintErr("Transporter Line is null");
        
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
        
        if (!CanFireWeapon())
            return;
        
        if (!ConsumePowerIfAvailable(ParentShip, 0))
            return;
        
        Line.Points = new Vector2[] { Line.ToLocal(ParentShip.GlobalPosition), Line.ToLocal(targetPosition) };
        Line.Visible = true;
        
        EffectTimer.Start();
        HitNearbyShip(targetPosition);
    }

    public void HitNearbyShip(Vector2 targetPosition)
    {
        Array<Node> nodes = GetTree().GetNodesInGroup("Ship");

        foreach (Node node in nodes)
        {
            if (node is Ship ship)
            {
                if (ship.GlobalPosition.DistanceTo(targetPosition) <= HitEffectRadius)
                {
                    if (ship == ParentShip)
                    {
                        continue;
                    }

                    if (!ship.HasShield())
                    {
                        if (new Random().Next(2) == 1)
                        {
                            ParentShip.Hull.CurrentMaterials += ship.Hull.CurrentMaterials * 0.1;
                            ParentShip.FusionReactor.CurrentFuel += ship.FusionReactor.CurrentFuel * 0.1;
                            
                            ship.Hull.CurrentMaterials = Mathf.Max(0, ship.Hull.CurrentMaterials * 0.9);
                            ship.FusionReactor.CurrentFuel = Mathf.Max(0, ship.FusionReactor.CurrentFuel * 0.9);
                            
                            GD.Print("Stole materials");
                            GD.Print("Reactor Fuel: " + ParentShip.FusionReactor.CurrentFuel);
                            GD.Print("Materials: " + ParentShip.Hull.CurrentMaterials);
                        }
                        
                        return;
                    }
                }
            }
        }
    }

    public void HideLine()
    {
        Line.Visible = false;
    }
}