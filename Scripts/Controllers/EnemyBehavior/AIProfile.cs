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
}