public partial class CoaxialWarpDrive : WarpDrive
{
    public override bool CanActivate()
    {
        return ParentShip.CurrentPower >= 
               CurrentConsumption * 1.5 && CurrentHealth > 0 &&
               ParentShip.Engine.GetHealthPercentage() >= 0.1;
    }
    
    public override void EnterAcceleration()
    {
        base.EnterAcceleration();
        ParentShip.DamageEngine(ParentShip.Engine.GenericStats.Health * 0.1);
    }
}