using Godot;

public partial class GenericWarpToPositionState : AIState
{
    [Export]
    public double MinWarpDistance;
    
    public override void EnterState(EnemyController controller, AIState previousState)
    {
        controller.MovementTarget = controller.PursuitTarget.GlobalPosition;
    }

    public override void Process(EnemyController controller, double delta)
    {
        if (controller.PursuitTarget is null)
            return;
        
        controller.MovementTarget = controller.PursuitTarget.GlobalPosition;

        if (controller.ParentShip.GlobalPosition.DistanceTo(controller.MovementTarget) <= MinWarpDistance)
            return;
        
        if(Mathf.Abs(controller.ParentShip.TurnToFace(controller.MovementTarget, delta)) < Mathf.DegToRad(2))
        {
            if (!controller.ParentShip.InWarp())
            {
                if (controller.ParentShip.CanWarp())
                    controller.ParentShip.WarpDrive.Toggle();
            }
        }
    }

    public override void PhysicsProcess(EnemyController controller, double delta)
    {
    }

    public override void ExitState(EnemyController controller, AIState nextState)
    {
        if (controller.ParentShip.WarpDrive is not null)
        {
            if (controller.ParentShip.WarpDrive.Active)
                controller.ParentShip.WarpDrive.Toggle();
        }
        
        controller.PursuitTarget = null;
    }
}