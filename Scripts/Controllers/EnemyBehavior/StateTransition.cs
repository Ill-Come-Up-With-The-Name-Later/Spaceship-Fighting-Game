using Godot;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;

public abstract partial class StateTransition : Resource
{
    [Export]
    public AIState From;
    
    [Export]
    public AIState To;

    public abstract bool ShouldTransition(EnemyController controller, double deltaTime);
}