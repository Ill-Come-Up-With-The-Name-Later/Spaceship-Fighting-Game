
public partial class GoToParent : AIState
{
    public override void EnterState(EnemyController controller, AIState previousState)
    {
        if (controller is DroneController drone)
        {
            if (drone.ParentShip is null)
                return;
            
            drone.PursuitTarget = drone.ParentShip;
        }
    }

    public override void Process(EnemyController controller, double delta)
    {
        if (!(controller is DroneController drone))
            return;

        if (drone.ParentShip is null)
            return;
        
        controller.MovementTarget = controller.PursuitTarget.GlobalPosition;
    }

    public override void PhysicsProcess(EnemyController controller, double delta)
    {
    }

    public override void ExitState(EnemyController controller, AIState nextState)
    {
    }
}