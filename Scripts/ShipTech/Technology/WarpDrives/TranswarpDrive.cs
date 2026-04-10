public partial class TranswarpDrive : WarpDrive
{
    public override bool CanActivate()
    {
        return ParentShip.CurrentPower >= 
            CurrentConsumption * 1.5 && CurrentHealth > 0 &&
            ParentShip.Engine.GetHealthPercentage() >= 0.5;
    }

    public override void ExitWarpState()
    {
        base.ExitWarpState();
        Eventbus.EmitSignal("TranswarpExited", ParentShip);
    }
}