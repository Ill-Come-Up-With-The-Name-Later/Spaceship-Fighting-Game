using Godot;

public partial class RoamToReturn : StateTransition
{
    [Export]
    public double minRecallDistance;
    
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        if (controller is DroneController drone)
        {
            if (drone.ParentShip is null)
                return false;
            
            return drone.ParentShip.GlobalPosition.DistanceTo(drone.GlobalPosition) >= minRecallDistance;
        }

        return false;
    }
}