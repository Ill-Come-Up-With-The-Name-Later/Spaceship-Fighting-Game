using Godot;

public partial class DroneMaterializer : Ability
{
    [Export]
    public PackedScene Drone;

    public bool DroneActive;
    
    public override void Use()
    {
        if(Drone is null)
            GD.PrintErr("Drone materializer has no drone");

        Drone drone = (Drone)Drone.Instantiate();
        
        GetTree().GetRoot().AddChild(drone);
        
        drone.GlobalPosition = ParentShip.GlobalPosition;
        DroneController droneController = (DroneController)drone.ControlNode;
        droneController.SpawningShip = ParentShip;
        drone.ParentMaterializer = this;
        drone.ControlNode.Faction = ParentShip.ControlNode.Faction;
        DroneActive = true;
    }
}
