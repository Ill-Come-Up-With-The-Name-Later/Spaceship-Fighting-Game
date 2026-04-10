using Godot;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;

/// <summary>
/// A conditional transition between two states.
/// Must be extended from to use.
/// </summary>
public abstract partial class StateTransition : Resource
{
    [Export]
    public AIState From;
    
    [Export]
    public AIState To;

    public abstract bool ShouldTransition(EnemyController controller, double deltaTime);
}