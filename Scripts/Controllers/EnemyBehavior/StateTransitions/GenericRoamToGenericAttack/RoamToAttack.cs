public partial class RoamToAttack : StateTransition
{
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        return controller.TargetingOrder.Count > 0 && !controller.TargetingOrder.Peek().ControlNode.Dead;
    }
}