using Godot;
using SpaceshipFightingGame.Scripts.Resources.Technology;
using SpaceshipFightingGame.Scripts.ShipTech;

public abstract partial class WarpDrive : ShipPart
{
    [Export]
    public WarpDriveData WarpDriveStats;

    public WarpDriveState WarpDriveState;

    public override void _Ready()
    {
        base._Ready();

        WarpDriveState = WarpDriveState.Inactive;
    }

    public override void _PhysicsProcess(double delta)
    {
    }

    public abstract void ActivateWarp();
    
    public abstract void DeactivateWarp();
}