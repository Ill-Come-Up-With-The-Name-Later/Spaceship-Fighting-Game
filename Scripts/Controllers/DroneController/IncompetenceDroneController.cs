using Godot;

public partial class IncompetenceDroneController : DroneController
{
    [Export]
    public double Lifespan;
    
    public override void _Process(double delta)
    {
        base._Process(delta);

        Lifespan -= delta;

        if (Lifespan <= 0)
        {
            ParentShip.Eventbus.EmitSignal("ShipDamaged",
                ParentShip, ParentShip, 2147483647, 2147483647, 2147483647);
        }
    }
}