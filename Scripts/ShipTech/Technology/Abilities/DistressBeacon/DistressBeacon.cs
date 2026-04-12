using Godot;

public partial class DistressBeacon : Ability
{
    [Export]
    public double Range;

    [Export]
    public double ActivationDelay;

    public Timer ActivationTimer;

    public override void _Ready()
    {
        base._Ready();
        ActivationTimer = GetNode<Timer>("ActivationTimer");
        
        if(ActivationTimer is null)
            GD.PrintErr("Activation timer not found");
        
        ActivationTimer.WaitTime = ActivationDelay;
        ActivationTimer.Timeout += Emit;
    }

    public override void _ExitTree()
    {
        ActivationTimer.Timeout -= Emit;
    }

    public override void Use()
    {
        ActivationTimer.Start();
    }

    public void Emit()
    {
        Eventbus.EmitSignal("DistressBeaconUsed", ParentShip, this);
        QueueFree();
    }
}