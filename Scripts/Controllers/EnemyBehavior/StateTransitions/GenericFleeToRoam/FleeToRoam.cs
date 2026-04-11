using Godot;

public partial class FleeToRoam : StateTransition
{
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        return controller.ParentShip.GlobalPosition.DistanceTo(controller.MovementTarget) <=
               controller.ParentShip.Velocity.Length() / 2;
    }
}