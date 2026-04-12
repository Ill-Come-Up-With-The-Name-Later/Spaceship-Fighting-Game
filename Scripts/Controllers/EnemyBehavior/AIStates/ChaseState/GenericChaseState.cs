using Godot;

public partial class GenericChaseState : AIState
{
    public override void EnterState(EnemyController controller, AIState previousState)
    {
        controller.MovementTarget = controller.PursuitTarget.GlobalPosition;
    }
    
    public override void Process(EnemyController controller, double delta)
    {
        controller.MovementTarget = controller.PursuitTarget.GlobalPosition;
        GD.Print("Distance to target: " + controller.ParentShip.GlobalPosition.DistanceTo(controller.MovementTarget));
        GD.Print("Angle difference: " + controller.ParentShip.GlobalPosition.AngleTo(controller.MovementTarget));
        
        if(Mathf.Abs(controller.TurnToFace(controller.MovementTarget, delta)) < Mathf.DegToRad(5))
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