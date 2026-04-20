public partial class UseTransporterState : AIState
{
    public override void EnterState(EnemyController controller, AIState previousState)
    {
        if (!IsInstanceValid(controller))
            return;

        if (controller.ParentShip.GetWeaponsOfType<Transporter>().Count == 0)
            return;
        
        if (controller.ParentShip.DefenseSystem.DefenseActive)
        {
            if(controller.ParentShip.DefenseSystem.CanToggle)
                controller.ParentShip.DefenseSystem.Toggle();
        }
    }

    public override void Process(EnemyController controller, double delta)
    {
        if (!controller.ParentShip.DefenseSystem.DefenseActive)
        {
            Ship target = controller.TargetingOrder.Peek();

            Transporter transporter = controller.ParentShip.GetWeaponsOfType<Transporter>()[0];

            if (!target.DefenseSystem.DefenseActive)
                controller.FireAt(transporter, target);
        }
    }

    public override void PhysicsProcess(EnemyController controller, double delta)
    {
    }

    public override void ExitState(EnemyController controller, AIState nextState)
    {
        if (!controller.ParentShip.DefenseSystem.DefenseActive)
        {
            if(controller.ParentShip.DefenseSystem.CanToggle)
                controller.ParentShip.DefenseSystem.Toggle();
        }
    }
}
