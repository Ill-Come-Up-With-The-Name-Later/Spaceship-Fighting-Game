using Godot;
using Godot.Collections;

public partial class GravityWell : Area2D
{
    [Export]
    public float GravityStrength;

    public BlackHole Parent;

    public override void _Ready()
    {
        Parent = GetParent<BlackHole>();
        
        if(Parent is null)
            GD.PrintErr("No parent body");

        AreaEntered += OnAreaEntered;
    }

    public override void _Process(double delta)
    {
        Array<Area2D> overlapping = GetOverlappingAreas();

        foreach (Area2D area in overlapping)
        {
            if (area is Hurtbox hurtbox)
            {
                Ship ship = hurtbox.ParentShip;
                Vector2 direction = (GlobalPosition - ship.GlobalPosition).Normalized();
                
                direction *= GravityStrength * (Mathf.Clamp(Mathf.Pow(Mathf.E, 
                    GlobalPosition.DistanceTo(ship.GlobalPosition)),0, 1));
                
                ship.Velocity += direction * (float)delta;
            }
        }
    }

    public void OnAreaEntered(Area2D area)
    {
        if (area is Hurtbox hurtbox)
        {
            GD.Print(hurtbox.ParentShip.GetName()+ " has entered the gravity well.");
        }
    }
}