using Godot;

public partial class EphemeralDroneController : DroneController
{
    [Export]
    public double FadeRate;
    
    public override void _Process(double delta)
    {
        base._Process(delta);

        var selfMod = ParentShip.Sprite.SelfModulate;
        selfMod.A -= (float)(FadeRate * delta);
        ParentShip.Sprite.SelfModulate = selfMod;

        if (selfMod.A <= 0)
            QueueFree();
    }
}