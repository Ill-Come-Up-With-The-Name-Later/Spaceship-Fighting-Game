using Godot;

public partial class AlcubierreWarpDrive : WarpDrive
{
    public override void ActivateWarp()
    {
        base.ActivateWarp();
        ParentShip.CanTurn = false;
    }

    public override void DeactivateWarp()
    {
        base.DeactivateWarp();
        ParentShip.CanTurn = true;
    }

    public override void Warping(double delta)
    {
        base.Warping(delta);
        
        GpuParticles2D bubbleClose = EnterExitParticles.Instantiate<GpuParticles2D>();
        GetTree().CurrentScene.AddChild(bubbleClose);
        
        bubbleClose.GlobalPosition = GlobalPosition - ParentShip.Velocity * (float)delta;
        bubbleClose.GlobalRotation = ParentShip.GlobalRotation + Mathf.Pi;
        bubbleClose.Restart();
        bubbleClose.Emitting = true;
        bubbleClose.Finished += QueueFree;
    }
}