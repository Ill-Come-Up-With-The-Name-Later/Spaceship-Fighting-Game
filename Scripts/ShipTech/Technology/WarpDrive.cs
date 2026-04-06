using Godot;
using SpaceshipFightingGame.Scripts.Resources.Technology;
using SpaceshipFightingGame.Scripts.ShipTech;

public partial class WarpDrive : ShipPart
{
    [Export]
    public WarpDriveData WarpDriveStats;

    public WarpDriveState WarpDriveState;
    public Timer WarpDriveTimer;
    public Timer AccelerationTimer;
    public Timer DecelerationTimer;

    public bool Active => WarpDriveState != WarpDriveState.Inactive;

    public override void _Ready()
    {
        base._Ready();

        WarpDriveState = WarpDriveState.Inactive;
        
        WarpDriveTimer = GetNode<Timer>("DriveTimer");
        WarpDriveTimer.WaitTime = WarpDriveStats.MaxDuration;
        WarpDriveTimer.Timeout += TransitionToDeceleration;
        
        AccelerationTimer = GetNode<Timer>("AccelerationTimer");
        AccelerationTimer.WaitTime = WarpDriveStats.AccelerationTime;
        AccelerationTimer.Timeout += TransitionToWarping;
        
        DecelerationTimer = GetNode<Timer>("DecelerationTimer");
        DecelerationTimer.WaitTime = WarpDriveStats.DecelerationTime;
        DecelerationTimer.Timeout += ExitWarpState;
    }

    public override void _ExitTree()
    {
        WarpDriveTimer.Timeout -= TransitionToDeceleration;
        AccelerationTimer.Timeout -= TransitionToWarping;
        DecelerationTimer.Timeout -= ExitWarpState;
    }

    public virtual void EnterAcceleration()
    {
        WarpDriveState = WarpDriveState.Accelerating;
        AccelerationTimer.Start();
    }

    public virtual void TransitionToWarping()
    {
        WarpDriveState = WarpDriveState.Warping;
        WarpDriveTimer.Start();
    }

    public virtual void TransitionToDeceleration()
    {
        WarpDriveState = WarpDriveState.Decelerating;
        DecelerationTimer.Start();
    }

    public virtual void ExitWarpState()
    {
        WarpDriveState = WarpDriveState.Inactive;
    }

    public override void _PhysicsProcess(double delta)
    {
    }

    public virtual void ActivateWarp()
    {
        WarpDriveTimer.WaitTime = WarpDriveStats.MaxDuration;
        AccelerationTimer.WaitTime = WarpDriveStats.AccelerationTime;
        DecelerationTimer.WaitTime = WarpDriveStats.DecelerationTime;
        
        EnterAcceleration();
    }

    public virtual void DeactivateWarp()
    {
        TransitionToDeceleration();
    }
}