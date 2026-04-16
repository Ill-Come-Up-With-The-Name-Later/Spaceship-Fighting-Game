public partial class UseDistressBeaconState : AIState
{
    public override void EnterState(EnemyController controller, AIState previousState)
    {
        if (controller.ParentShip.HasAbility<DistressBeacon>())
        {
            DistressBeacon beacon = controller.ParentShip.GetAbility<DistressBeacon>();
            
            if(beacon.CanUse)
                beacon.Use();
        }
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