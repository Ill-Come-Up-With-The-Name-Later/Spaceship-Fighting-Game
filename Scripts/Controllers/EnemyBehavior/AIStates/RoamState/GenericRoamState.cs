using Godot;

public partial class GenericRoamState : AIState
{
    [Export]
    public float RoamRange;

    [Export]
    public double TimeBetweenRetargets;
    
    public override void EnterState(EnemyController controller, AIState previousState)
    {
    }

    public override void Process(EnemyController controller, double delta)
    {
        controller.ManipulableCounter += delta;

        if (controller.ManipulableCounter >= TimeBetweenRetargets && controller.CloseToMoveTarget())
        {
            SetNewTarget(controller);
            controller.ManipulableCounter = 0;
        }
    }

    public override void PhysicsProcess(EnemyController controller, double delta)
    {
    }

    public override void ExitState(EnemyController controller, AIState nextState)
    {
    }
    
    private void SetNewTarget(EnemyController controller)
    {
        RandomNumberGenerator generator = new RandomNumberGenerator();
        
        Vector2 offset = new Vector2(generator.RandfRange(-RoamRange, RoamRange), 
            generator.RandfRange(-RoamRange, RoamRange));

        controller.MovementTarget = controller.ParentShip.GlobalPosition + offset;
    }
}