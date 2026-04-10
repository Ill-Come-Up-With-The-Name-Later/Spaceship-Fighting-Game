using Godot;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;

/// <summary>
/// An AI state. Must be extended from
/// to use.
/// </summary>
public abstract partial class AIState : Resource
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
    public abstract void EnterState(EnemyController controller, AIState previousState);

    /// <summary>
    /// Equivalent of _Process but fo AI states
    /// </summary>
    /// <param name="controller">
    /// The enemy
    /// </param>
    /// <param name="delta">
    /// Delta time
    /// </param>
    public abstract void Process(EnemyController controller, double delta);

    /// <summary>
    /// Equivalent of _PhysicsProcess for AI states
    /// </summary>
    /// <param name="controller">
    /// The enemy
    /// </param>
    /// <param name="delta">
    /// Delta time
    /// </param>
    public abstract void PhysicsProcess(EnemyController controller, double delta);

    /// <summary>
    /// Triggers when the state is exited
    /// </summary>
    /// <param name="controller">
    /// The enemy exiting the state
    /// </param>
    /// <param name="nextState">
    /// The state being transitioned to
    /// </param>
    public abstract void ExitState(EnemyController controller, AIState nextState);
}
