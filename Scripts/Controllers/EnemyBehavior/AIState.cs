using Godot;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;

public partial class AIState : Resource
{
    [Export]
    public AIStateType Type;
    
    /// <summary>
    /// Triggers when the state is entered
    /// </summary>
    /// <param name="controller">
    /// The enemy entering the state
    /// </param>
    /// <param name="previousState">
    /// The previous AIState
    /// </param>
    public virtual void EnterState(EnemyController controller, AIState previousState)
    {
    }

    /// <summary>
    /// Equivalent of _Process but fo AI states
    /// </summary>
    /// <param name="controller">
    /// The enemy
    /// </param>
    /// <param name="delta">
    /// Delta time
    /// </param>
    public virtual void Process(EnemyController controller, double delta)
    {
    }

    /// <summary>
    /// Equivalent of _PhysicsProcess for AI states
    /// </summary>
    /// <param name="controller">
    /// The enemy
    /// </param>
    /// <param name="delta">
    /// Delta time
    /// </param>
    public virtual void PhysicsProcess(EnemyController controller, double delta)
    {
    }

    /// <summary>
    /// Triggers when the state is exited
    /// </summary>
    /// <param name="controller">
    /// The enemy exiting the state
    /// </param>
    /// <param name="nextState">
    /// The state being transitioned to
    /// </param>
    public virtual void ExitState(EnemyController controller, AIState nextState)
    {
    }
}
