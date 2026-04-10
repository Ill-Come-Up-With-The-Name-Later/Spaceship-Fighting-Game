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

    public Timer CooldownTimer;

    protected int Counter;

    protected int AccelerationFrames;
    protected int DecelerationFrames;

    public bool Active => WarpDriveState != WarpDriveState.Inactive;

    public bool CanToggle { get; set; }
    
    public Eventbus Eventbus;

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
        
        CooldownTimer = GetNode<Timer>("CooldownTimer");
        CooldownTimer.WaitTime = WarpDriveStats.Cooldown;
        CooldownTimer.Timeout += EndCooldown;

        CurrentConsumption = PowerConsumption.Amount;
        ConsumingPower = false;
        
        Counter = 0;
        AccelerationFrames = (int)(60 * WarpDriveStats.AccelerationTime);
        DecelerationFrames = (int)(60 * WarpDriveStats.DecelerationTime);
        
        Eventbus = GetNode<Eventbus>("/root/Eventbus");
        
        if(Eventbus is null)
            GD.PrintErr("Eventbus was not found");

        CanToggle = true;
    }

    public void EndCooldown()
    {
        CanToggle = true;
    }

    public override void _ExitTree()
    {
        WarpDriveTimer.Timeout -= TransitionToDeceleration;
        AccelerationTimer.Timeout -= TransitionToWarping;
        DecelerationTimer.Timeout -= ExitWarpState;
        CooldownTimer.Timeout -= EndCooldown;
    }

    /// <summary>
    /// Starts the warp drive's acceleration
    /// </summary>
    public virtual void EnterAcceleration()
    {
        GD.Print("Started warping with " + GetType());
        
        WarpDriveState = WarpDriveState.Accelerating;
        AccelerationTimer.Start();
        ParentShip.CanMove = false;
        ConsumingPower = true;
    }

    /// <summary>
    /// Changes the warp drive to its main
    /// warping
    /// </summary>
    public virtual void TransitionToWarping()
    {
        GD.Print("In main warp");
        
        WarpDriveState = WarpDriveState.Warping;
        WarpDriveTimer.Start();
    }

    /// <summary>
    /// Changes the warp drive into decelerating
    /// </summary>
    public virtual void TransitionToDeceleration()
    {
        GD.Print("Decelerating");
        
        WarpDriveState = WarpDriveState.Decelerating;
        AccelerationTimer.Stop();
        DecelerationTimer.Start();
    }

    /// <summary>
    /// Ends the warping and deceleration
    /// </summary>
    public virtual void ExitWarpState()
    {
        GD.Print("Ended warp with " + GetType());
        
        WarpDriveState = WarpDriveState.Inactive;
        ParentShip.Velocity = Vector2.Zero;
        ParentShip.CanMove = true;
        ConsumingPower = false;
        
        Eventbus.EmitSignal("ShipLeaveWarp", ParentShip);
        CanToggle = false;
        CooldownTimer.Start();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Active)
        {
            double speedMultiplier = 1;
            
            if(!(ConsumePowerIfAvailable(ParentShip, delta) && CanActivate()))
                TransitionToDeceleration();

            if (WarpDriveState == WarpDriveState.Accelerating || WarpDriveState == WarpDriveState.Decelerating)
            {
                Counter++;
                CurrentConsumption = PowerConsumption.Amount * 0.2;
            }

            if (WarpDriveState == WarpDriveState.Accelerating)
                speedMultiplier = Mathf.Pow(WarpDriveStats.SpeedMultiplier, 
                    (double)Counter / AccelerationFrames);
            else if (WarpDriveState == WarpDriveState.Decelerating)
                speedMultiplier = Mathf.Pow(1.0 / WarpDriveStats.SpeedMultiplier, 
                    (double)Counter / AccelerationFrames);
            else if (WarpDriveState == WarpDriveState.Warping)
            {
                speedMultiplier = WarpDriveStats.SpeedMultiplier;
                CurrentConsumption = PowerConsumption.Amount;
                
                GD.Print("Warp velocity: " + ParentShip.Velocity.Length());
            }
            
            ParentShip.Velocity =
                (Vector2.Up * (float)ParentShip.Engine.TrueMoveSpeed() * (float)speedMultiplier).Rotated(ParentShip
                    .GlobalRotation);
        }
    }

    /// <summary>
    /// If the warp drive can be activated.
    /// Override if the drive needs some special condition
    /// like a separate resource or some health amount
    /// </summary>
    /// <returns>
    /// If the warp drive can be activated
    /// </returns>
    public virtual bool CanActivate()
    {
        return ParentShip.CurrentPower >= CurrentConsumption * 1.5
               && CurrentHealth > 0 && ParentShip.Engine.CurrentHealth > 0;
    }

    /// <summary>
    /// Activates warp and starts acceleration
    /// </summary>
    public virtual void ActivateWarp()
    {
        WarpDriveTimer.WaitTime = WarpDriveStats.MaxDuration;
        AccelerationTimer.WaitTime = WarpDriveStats.AccelerationTime;
        DecelerationTimer.WaitTime = WarpDriveStats.DecelerationTime;
        Counter = 0;
        
        EnterAcceleration();
        Eventbus.EmitSignal("ShipEnterWarp", ParentShip);
    }

    public virtual void DeactivateWarp()
    {
        Counter = 0;
        
        TransitionToDeceleration();
    }
    
    public bool ConsumePowerIfAvailable(Ship ship, double delta)
    {
        if (CurrentConsumption * delta > ship.CurrentPower)
            return false;
        
        ship.FusionReactor.CurrentPower -= CurrentConsumption * delta;

        return true;
    }

    public void Toggle()
    {
        if (!CanToggle)
            return;

        if (ParentShip.Cloaked())
            return;
        
        if (Active)
            DeactivateWarp();
        else
            if (CanActivate()) 
                ActivateWarp();
    }
}