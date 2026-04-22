using Godot;

public partial class DistressBeacon : Ability
{
    [Export]
    public double Range;

    [Export]
    public double ActivationDelay;

    public Timer ActivationTimer;

    public bool CanUse;

    [Export]
    public PackedScene DistressBeaconParticles;

    public override void _Ready()
    {
        base._Ready();
        ActivationTimer = GetNode<Timer>("ActivationTimer");
        
        if(ActivationTimer is null)
            GD.PrintErr("Activation timer not found");
        
        ActivationTimer.WaitTime = ActivationDelay;
        ActivationTimer.Timeout += Emit;

        CanUse = true;
    }

    public override void _ExitTree()
    {
        ActivationTimer.Timeout -= Emit;
    }

    public override void Use()
    {
        if (!CanUse)
            return;
        
        GD.Print("Using distress beacon");
        
        ActivationTimer.Start();
        CanUse = false;
    }

    public void Emit()
    {
        if (ParentShip is not null && !ParentShip.ControlNode.Dead)
        {
            GD.Print("Emitting distress signal");
            
            GpuParticles2D transmit = DistressBeaconParticles.Instantiate<GpuParticles2D>();
            GetTree().CurrentScene.AddChild(transmit);

            transmit.GlobalPosition = GlobalPosition;
            transmit.Restart();
            transmit.Emitting = true;
            transmit.Finished += QueueFree;
            
            Eventbus.EmitSignal("DistressBeaconUsed", ParentShip, this);
            QueueFree();
        }
    }
}