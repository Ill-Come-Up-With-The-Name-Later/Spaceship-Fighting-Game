public partial class AttackToUseBeacon : StateTransition
{
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        if (!controller.ParentShip.HasAbility<DistressBeacon>())
            return false;
        
        DistressBeacon beacon = controller.ParentShip.GetAbility<DistressBeacon>();

        if (!beacon.CanUse)
            return false;
        
        return !controller.ParentShip.DefenseSystem.DefenseActive &&
               controller.ParentShip.GetMissingHealthPercentage() >= 0.2;
    }
}