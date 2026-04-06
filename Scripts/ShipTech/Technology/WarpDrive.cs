using Godot;
using SpaceshipFightingGame.Scripts.Resources.Technology;
using SpaceshipFightingGame.Scripts.ShipTech;

public abstract partial class WarpDrive : ShipPart
{
    [Export]
    public WarpDriveData WarpDriveStats;

    public bool WarpActive;

    public override void _Ready()
    {
        base._Ready();
        
        WarpActive = false;
    }

    public override void _PhysicsProcess(double delta)
    {
    }

    public abstract void ActivateWarp();
    
    public abstract void DeactivateWarp();
}