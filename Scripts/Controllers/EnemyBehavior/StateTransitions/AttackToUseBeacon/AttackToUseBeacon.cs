public partial class AttackToUseBeacon : StateTransition
{
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        return !controller.ParentShip.DefenseSystem.DefenseActive &&
               controller.ParentShip.GetMissingHealthPercentage() >= 0.3;
    }
}