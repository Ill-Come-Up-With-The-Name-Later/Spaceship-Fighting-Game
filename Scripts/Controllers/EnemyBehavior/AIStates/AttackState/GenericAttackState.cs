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

        if (controller.ParentShip.WarpDrive.Active)
            controller.ParentShip.WarpDrive.Toggle();
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

        if (target.Velocity.Length() <= controller.ParentShip.Velocity.Length())
        {
            Vector2 behind = -target.Forward;
            behind *= (float)controller.FiringRange * 0.8f;
            
            controller.MovementTarget = target.GlobalPosition + behind;
            controller.ParentShip.TurnToFace(target.GlobalPosition, delta);
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
            if(controller.Forward.Dot(target.Forward) >
               Mathf.Cos(Mathf.DegToRad(controller.ParentShip.Weapons.AttackFOV)))
                controller.ParentShip.TurnToFace(target.GlobalPosition, delta);
            
            RandomNumberGenerator rand = new RandomNumberGenerator();
            
            Vector2 offset = new Vector2(rand.RandfRange(-(float)controller.FiringRange * 0.75f,
                    (float)controller.FiringRange * 0.75f),
                rand.RandfRange(-(float)controller.FiringRange * 0.75f,
                    (float)controller.FiringRange * 0.75f));

            if (rand.Randf() < 0.075)
                offset *= 0.2f;

            controller.MovementTarget = target.GlobalPosition + offset;
            
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
        if (nextState.Type == AIStateType.Chase)
        {
            controller.TargetingOrder.TryPeek(out Ship target, out double priority);
            
            if (target is not null && controller.ParentShip.CanTarget(target) && !target.Cloaked())
            {
                if (!IsInstanceValid(target))
                    return;
                
                controller.MovementTarget = target.GlobalPosition;
                controller.PursuitTarget = target;
            }
        }
    }
}
