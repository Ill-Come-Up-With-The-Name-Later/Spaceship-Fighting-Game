using Godot;
using Godot.Collections;

/// <summary>
/// A type of AI, has states in it already
/// </summary>
public partial class AIProfile : Resource
{
    /// <summary>
    /// All AI States
    /// </summary>
    [Export]
    public Array<AIState> AIStates;

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
    /// Dictionary of states to their names
    /// </summary>
    private Dictionary<AIStateType, AIState> StateMap = new();

    public void Initialize()
    {
        StateMap.Clear();

        foreach (var state in AIStates)
        {
            if (state == null)
                continue;

            if (StateMap.ContainsKey(state.Type))
            {
                GD.PrintErr("AIState duplicate type: " + state.Type);
                continue;
            }

            StateMap[state.Type] = state;
        }
    }
    
    public AIState GetState(AIStateType type)
    {
        return StateMap.TryGetValue(type, out var state) ? state : null;
    }
}