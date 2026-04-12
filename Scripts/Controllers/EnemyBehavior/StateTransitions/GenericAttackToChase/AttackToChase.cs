public partial class AttackToChase : StateTransition
{
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        controller.TargetingOrder.TryPeek(out Ship target, out double priority);

        if (target is not null)
        {
            if (target.Cloaked() && !controller.ParentShip.HasUpgrade<AntiCloakScanner>())
                return false;
            
            if (target.ControlNode is EnemyController targetController)
            {
                if(targetController.CurrentState.Type == AIStateType.Flee)
                    return true;
            }
            else
            {
                if(target.InWarp() && !(target.ControlNode is EnemyController))
                    return true;
            }
        }

        return false;
    }
}