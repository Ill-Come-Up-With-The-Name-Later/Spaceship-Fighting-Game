public partial class WarpToPositionToAttack : StateTransition
{
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        if (controller.PursuitTarget is null)
            return true;
        
        return controller.ParentShip.GlobalPosition.DistanceTo(controller.MovementTarget)
            <= controller.ParentShip.Velocity.Length() / 2 || controller.TimeInState >= 10;
    }
}