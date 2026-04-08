using Godot;

public partial class Phaser : Weapon
{
    [Export]
    public double Range;
    
    public Line2D Line;
    public RayCast2D Ray;

    public Timer EffectTimer;

    public override void _Ready()
    {
        base._Ready();
        
        Line = GetNode<Line2D>("Line");
        Line.Visible = false;
        
        Ray = GetNode<RayCast2D>("RayCast");
        
        EffectTimer = GetNode<Timer>("EffectTimer");
        EffectTimer.Timeout += HideLine;
    }
    
    public override void Fire(Vector2 targetPosition)
    {
        base.Fire(targetPosition);
        
        Line.Points = new Vector2[] { Line.ToLocal(ParentShip.GlobalPosition), Line.ToLocal(targetPosition) };
        Line.Visible = true;
        
        EffectTimer.Start();
    }

    public void HideLine()
    {
        Line.Visible = false;
    }
}