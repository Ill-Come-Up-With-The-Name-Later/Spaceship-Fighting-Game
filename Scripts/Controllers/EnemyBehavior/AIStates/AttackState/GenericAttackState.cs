using System.Collections.Generic;
using Godot;

public partial class GenericAttackState : AIState
{
    public override void EnterState(EnemyController controller, AIState previousState)
    {
    }

    public override void Process(EnemyController controller, double delta)
    {
        controller.ManipulableCounter++;
        
        Vector2 offset = new Vector2((float)Mathf.Cos(controller.ManipulableCounter),
            (float)Mathf.Sin(controller.ManipulableCounter));

        offset *= (float)controller.FiringRange * 0.85f;
        
        controller.MovementTarget = controller.TargetingOrder.Peek().GlobalPosition + offset;
    }

    public override void PhysicsProcess(EnemyController controller, double delta)
    {
    }

    public override void ExitState(EnemyController controller, AIState nextState)
    {
    }
}
