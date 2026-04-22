public partial class RoamToAttack : StateTransition
{
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        if (controller.TargetingBuffer.Count > 0)
        {
            controller.TargetingBuffer.TryPeek(out Ship tempTarget, out double priority);

            if (tempTarget != null)
            {
                controller.TargetingOrder.Enqueue(tempTarget, priority);
                return true;
            }
        }

        return controller.TargetingOrder.Count > 0 && !controller.TargetingOrder.Peek().ControlNode.Dead;
    }
}