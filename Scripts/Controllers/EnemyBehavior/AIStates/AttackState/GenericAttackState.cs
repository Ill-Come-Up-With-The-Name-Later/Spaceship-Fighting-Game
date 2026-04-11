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

        if (controller.TargetingOrder.Count == 0)
            return;
        
        Ship target = controller.TargetingOrder.Peek();
        
        SetMovementTarget(controller, target);
        Attack(controller, target);
    }

    public void SetMovementTarget(EnemyController controller, Ship target)
    {
        Vector2 offset = new Vector2((float)Mathf.Cos(controller.ManipulableCounter),
            (float)Mathf.Sin(controller.ManipulableCounter));

        offset *= (float)controller.FiringRange * 0.85f;
        
        if (!IsInstanceValid(target))
            return;
        
        controller.MovementTarget = controller.TargetingOrder.Peek().GlobalPosition + offset;
    }

    public void Attack(EnemyController controller, Ship target)
    {
        if (!IsInstanceValid(target))
            return;
        
        if(controller.CanAttack)
        {
            PriorityQueue<Weapon, double> weapons = controller.WeaponPriorities();

            if (weapons.Count == 0)
                return;

            if (controller.FireAt(weapons.Peek(), target))
            {
                weapons.Dequeue();
            }
        }
    }

    public override void PhysicsProcess(EnemyController controller, double delta)
    {
    }

    public override void ExitState(EnemyController controller, AIState nextState)
    {
    }
}
