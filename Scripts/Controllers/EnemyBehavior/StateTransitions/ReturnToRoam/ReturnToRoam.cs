using Godot;

public partial class ReturnToRoam : StateTransition
{
    [Export]
    public double proximity;
    
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        if (controller is DroneController drone)
        {
            if (drone.ParentShip is null)
                return true;
            
            return drone.ParentShip.GlobalPosition.DistanceTo(drone.GlobalPosition) <= proximity;
        }

        return true;
    }
}