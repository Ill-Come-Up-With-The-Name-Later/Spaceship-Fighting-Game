using Godot;
using SpaceshipFightingGame.Scripts.Resources.Technology;
using SpaceshipFightingGame.Scripts.ShipTech;

public partial class WarpDrive : ShipPart, IPowerConsumer, IToggleable
{
    [Export]
    public WarpDriveData WarpDriveStats;

    [Export]
    public PowerConsumptionData PowerConsumption { get; set; }

    public double CurrentConsumption { get; set; }
    
    public bool ConsumingPower { get; set; }

    public WarpDriveState WarpDriveState;
    public Timer WarpDriveTimer;
    public Timer AccelerationTimer;
    public Timer DecelerationTimer;

    protected int Counter;

    protected int AccelerationFrames;
    protected int DecelerationFrames;

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

        CurrentConsumption = PowerConsumption.Amount;
        ConsumingPower = false;
        
        Counter = 0;
        AccelerationFrames = (int)(60 * WarpDriveStats.AccelerationTime);
        DecelerationFrames = (int)(60 * WarpDriveStats.DecelerationTime);
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
        ParentShip.CanMove = false;
        ConsumingPower = true;
    }

    public virtual void TransitionToWarping()
    {
        WarpDriveState = WarpDriveState.Warping;
        WarpDriveTimer.Start();
    }

    public virtual void TransitionToDeceleration()
    {
        WarpDriveState = WarpDriveState.Decelerating;
        AccelerationTimer.Stop();
        DecelerationTimer.Start();
    }

    public virtual void ExitWarpState()
    {
        WarpDriveState = WarpDriveState.Inactive;
        ParentShip.Velocity = Vector2.Zero;
        ParentShip.CanMove = true;
        ConsumingPower = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Active)
        {
            double speedMultiplier = 1;
            
            if(!ConsumePowerIfAvailable(ParentShip, delta) || ParentShip.CurrentPower() < CurrentConsumption * 1.5)
                TransitionToDeceleration();

            if (WarpDriveState == WarpDriveState.Accelerating || WarpDriveState == WarpDriveState.Decelerating)
            {
                Counter++;
                CurrentConsumption = PowerConsumption.Amount *  0.2;
            }

            if (WarpDriveState == WarpDriveState.Accelerating)
                speedMultiplier = Mathf.Pow(WarpDriveStats.SpeedMultiplier, 
                    (double)Counter / AccelerationFrames);
            else if (WarpDriveState == WarpDriveState.Decelerating)
                speedMultiplier = Mathf.Pow(1 / WarpDriveStats.SpeedMultiplier, 
                    (double)Counter / AccelerationFrames);
            else if (WarpDriveState == WarpDriveState.Warping)
            {
                speedMultiplier = WarpDriveStats.SpeedMultiplier;
                CurrentConsumption = PowerConsumption.Amount;
            }

            ParentShip.Velocity =
                (Vector2.Up * (float)ParentShip.Engine.TrueMoveSpeed() * (float)speedMultiplier).Rotated(ParentShip
                    .GlobalRotation);
        }

        if (Input.IsActionJustPressed("toggle_warp_drive"))
        {
            Toggle();
        }
    }

    public virtual void ActivateWarp()
    {
        WarpDriveTimer.WaitTime = WarpDriveStats.MaxDuration;
        AccelerationTimer.WaitTime = WarpDriveStats.AccelerationTime;
        DecelerationTimer.WaitTime = WarpDriveStats.DecelerationTime;
        Counter = 0;
        
        EnterAcceleration();
    }

    public virtual void DeactivateWarp()
    {
        Counter = 0;
        
        TransitionToDeceleration();
    }
    
    public bool ConsumePowerIfAvailable(Ship ship, double delta)
    {
        if (CurrentConsumption > ship.CurrentPower())
            return false;
        
        ship.FusionReactor.CurrentPower -= CurrentConsumption * delta;

        return true;
    }

    public void Toggle()
    {
        if (Active)
            DeactivateWarp();
        else
            if (ParentShip.CurrentPower() >= CurrentConsumption * 1.5) 
                ActivateWarp();
    }
}