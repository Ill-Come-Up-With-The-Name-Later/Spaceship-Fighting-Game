public partial class AttackToRoam : StateTransition
{
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        return controller.TargetingOrder is null || controller.TargetingOrder.Count == 0;
    }
}
