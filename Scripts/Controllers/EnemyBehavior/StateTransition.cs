using Godot;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;

/// <summary>
/// A conditional transition between two states.
/// Must be extended from to use.
/// </summary>
public abstract partial class StateTransition : Resource
{
    /// <summary>
    /// The starting state
    /// </summary>
    [Export]
    public AIState From;
    
    /// <summary>
    /// The ending state
    /// </summary>
    [Export]
    public AIState To;

    /// <summary>
    /// If the state should transition
    /// </summary>
    /// <param name="controller">
    /// The enemy
    /// </param>
    /// <param name="deltaTime">
    /// Delta time
    /// </param>
    /// <returns>
    /// If a state transition should occur
    /// </returns>
    public abstract bool ShouldTransition(EnemyController controller, double deltaTime);
}