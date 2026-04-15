using Godot;
using Godot.Collections;

/// <summary>
/// A type of AI, has states in it already
/// </summary>
public partial class AIProfile : Resource
{
    /// <summary>
    /// The starting state
    /// </summary>
    [Export]
    public AIState InitialState;
    
    /// <summary>
    /// The transitions between states
    /// </summary>
    [Export]
    public Array<StateTransition> Transitions;
    
    /// <summary>
    /// Delay between attack volleys
    /// </summary>
    [Export]
    public double AttackDelay;

    /// <summary>
    /// The delay between re-sequencing
    /// targets
    /// </summary>
    [Export] 
    public double TargetingDelay;

    /// <summary>
    /// The delay between each individual
    /// attack firing
    /// </summary>
    [Export] 
    public double PerAttackDelay;

    /// <summary>
    /// Gets the first AI state by its type
    /// </summary>
    /// <param name="type">
    /// An AI state type
    /// </param>
    /// <returns>
    /// An AI state or null if none is found
    /// </returns>
    public AIState FindState(AIStateType type)
    {
        foreach(StateTransition transition in Transitions)
        {
            if(transition.From.Type == type)
                return transition.From;
            
            if(transition.To.Type == type)
                return transition.To;
        }

        return null;
    }
}