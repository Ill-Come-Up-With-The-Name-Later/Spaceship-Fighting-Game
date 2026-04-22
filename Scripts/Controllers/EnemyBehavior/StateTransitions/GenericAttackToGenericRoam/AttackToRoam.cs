public partial class AttackToRoam : StateTransition
{
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        if (controller.TargetingBuffer.Count > 0)
            return false;
        
        if (controller.TargetingOrder.Count > 0)
        {
            if (controller.TargetingOrder.Peek().ControlNode.Dead)
                return true;
        }

        return controller.TargetingOrder is null || controller.TargetingOrder.Count == 0;
    }
}
