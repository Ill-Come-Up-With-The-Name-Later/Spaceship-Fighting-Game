using System.Collections.Generic;
using Godot;

public partial class GenericAttackState : AIState
{
    public override void EnterState(EnemyController controller, AIState previousState)
    {
        if (!controller.ParentShip.HasShield())
        {
            if(controller.ParentShip.DefenseSystem.CanToggle)
                controller.ParentShip.DefenseSystem.Toggle();
        }
    }

    public override void Process(EnemyController controller, double delta)
    {
        controller.ManipulableCounter++;

        if (controller.TargetingOrder.Count == 0)
            return;
        
        Ship target = controller.TargetingOrder.Peek();
        
        SetMovementTarget(controller, target, delta);
        Attack(controller, target, delta);
    }

    public void SetMovementTarget(EnemyController controller, Ship target, double delta)
    {
        if (!IsInstanceValid(target))
            return;

        if (target.Velocity.Length() <= controller.ParentShip.Engine.EngineStats.TurningRadius)
        {
            Vector2 behind = -target.Forward;
            behind *= (float)controller.FiringRange * 0.8f;
            
            controller.MovementTarget = target.GlobalPosition + behind;
            
            if(controller.CloseToMoveTarget())
                controller.TurnToFace(target.GlobalPosition, delta);
        }
        else
        {
            Vector2 offset = new Vector2((float)Mathf.Cos(controller.ManipulableCounter),
                (float)Mathf.Sin(controller.ManipulableCounter)).Normalized();

            offset *= (float)controller.FiringRange * 0.95f;

            controller.MovementTarget = target.GlobalPosition + offset;
        }
    }

    public void Attack(EnemyController controller, Ship target, double delta)
    {
        if (!IsInstanceValid(target))
            return;
        
        if(controller.CanAttack)
        {
            RandomNumberGenerator rand = new RandomNumberGenerator();

            if (target.Velocity.Length() >= controller.ParentShip.Engine.EngineStats.TurningRadius)
            {
                Vector2 offset = new Vector2(rand.RandfRange(-(float)controller.FiringRange * 0.5f,
                        (float)controller.FiringRange * 0.5f),
                    rand.RandfRange(-(float)controller.FiringRange * 0.5f,
                        (float)controller.FiringRange * 0.5f));

                controller.MovementTarget = target.GlobalPosition + offset;
            }

            PriorityQueue<Weapon, double> weapons = controller.WeaponPriorities();

            if (weapons.Count == 0)
                return;

            if (controller.FireAt(weapons.Peek(), target))
                weapons.Dequeue();

            if (weapons.Count > 0 && controller.TargetingOrder.Count > 1)
                controller.TargetingOrder.Dequeue();
        }
        else
        {
            SetMovementTarget(controller, target, delta);
        }
    }

    public override void PhysicsProcess(EnemyController controller, double delta)
    {
    }

    public override void ExitState(EnemyController controller, AIState nextState)
    {
    }
}
