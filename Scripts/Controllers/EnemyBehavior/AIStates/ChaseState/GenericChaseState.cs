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
        
        if(controller.ParentShip.Forward.Dot(controller.PursuitTarget.Forward.Normalized()) < 0.985)
        {
            if(!controller.ParentShip.InWarp())
                controller.ParentShip.TurnToFace(controller.MovementTarget, delta);
        }
        else if (!controller.ParentShip.InWarp())
        {
            if (controller.ParentShip.CanWarp())
                controller.ParentShip.WarpDrive.Toggle();
        }

        if (controller.ParentShip.Velocity.Length() > controller.PursuitTarget.Velocity.Length()
            && controller.PursuitTarget.InWarp() 
            && controller.ParentShip.GlobalPosition.DistanceTo(controller.MovementTarget) 
            < controller.ParentShip.Velocity.Length() * 0.15)
        {
            controller.ParentShip.Velocity = controller.ParentShip.Velocity
                .Normalized() * controller.PursuitTarget.Velocity.Length();
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