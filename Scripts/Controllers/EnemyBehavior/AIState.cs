using Godot;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;

public partial class AIState : Resource
{
    public virtual void EnterState(EnemyController controller, AIState previousState)
    {
    }

    public virtual void Process(EnemyController controller, double delta)
    {
    }

    public virtual void PhysicsProcess(EnemyController controller, float delta)
    {
    }

    public virtual void ExitState(EnemyController controller, AIState nextState)
    {
    }
}
