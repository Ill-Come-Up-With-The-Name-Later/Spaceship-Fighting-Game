public partial class ChaseToAttack : StateTransition
{
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        return controller.ParentShip.GlobalPosition.DistanceTo(controller.MovementTarget)
               <= controller.ParentShip.Velocity.Length() / 2 || controller.TimeInState >= 15;
    }
}