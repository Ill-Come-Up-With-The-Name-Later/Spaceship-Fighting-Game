public partial class ChaseToAttack : StateTransition
{
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        if (!IsInstanceValid(controller.PursuitTarget))
            return true;
        
        if (controller.PursuitTarget is null)
            return true;

        if (controller.PursuitTarget.InWarp())
            return controller.TimeInState >= 15;

        return controller.ParentShip.GlobalPosition.DistanceTo(controller.MovementTarget)
               <= controller.ParentShip.Velocity.Length() / 2 || controller.TimeInState >= 15;
    }
}