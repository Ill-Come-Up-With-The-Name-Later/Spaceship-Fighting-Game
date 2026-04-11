using Godot;

public partial class GenericFleeState : AIState
{
    [Export]
    public double FleeDistance;
    
    public override void EnterState(EnemyController controller, AIState previousState)
    {
        controller.MovementTarget += controller.Forward * (float)FleeDistance;

        if (controller.ParentShip.GetTechnology<CloakingDrive>() is not null)
        {
            CloakingDrive drive = controller.ParentShip.GetTechnology<CloakingDrive>();

            if (drive.CanToggle)
            {
                drive.Toggle();
                return;
            }
        }

        if (controller.ParentShip.CanWarp())
            controller.ParentShip.WarpDrive.Toggle();
    }

    public override void Process(EnemyController controller, double delta)
    {
    }

    public override void PhysicsProcess(EnemyController controller, double delta)
    {
    }

    public override void ExitState(EnemyController controller, AIState nextState)
    {
    }
}