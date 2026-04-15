using Godot;
using Godot.Collections;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;

public partial class CollectiveManager : Node
{
    public Array<Ship> CollectiveShips;
    public Eventbus Eventbus;
    
    public override void _EnterTree()
    {
        CollectiveShips = new Array<Ship>();
    }

    public override void _Ready()
    {
        Eventbus = GetNode<Eventbus>("/root/Eventbus");
        
        if(Eventbus is null)
            GD.PrintErr("Eventbus not found");
        
        Eventbus.DistressBeaconUsed += RespondToDistressBeacon;
        Eventbus.ShipSpawned += ReceiveSpawn;
    }

    /// <summary>
    /// Deals with ship spawning and hooking in
    /// ships to the collective
    /// </summary>
    /// <param name="ship">
    /// The ship that has spawned
    /// </param>
    public void ReceiveSpawn(Ship ship)
    {
        if(ship.ControlNode.Faction != Faction.Collective)
            return;
        
        CollectiveShips.Add(ship);
    }
    
    /// <summary>
    /// Gets collective ships near a position
    /// </summary>
    /// <param name="pos">
    /// The position
    /// </param>
    /// <param name="distance">
    /// The maximum distance
    /// </param>
    /// <returns>
    /// An array of nearby ships
    /// </returns>
    public Array<Ship> NearbyShips(Vector2 pos, double distance)
    {
        Array<Ship> nearby = new Array<Ship>();
        
        foreach (Ship ship in CollectiveShips)
        {
            if(ship.GlobalPosition.DistanceTo(pos) <= distance)
                nearby.Add(ship);
        }
        
        return nearby;
    }

    public void RespondToDistressBeacon(Ship user, DistressBeacon used)
    {
        if (!CollectiveShips.Contains(user) || user.ControlNode is not EnemyController)
            return;

        Array<Ship> nearby = NearbyShips(user.GlobalPosition, used.Range);

        foreach (Ship ship in nearby)
        {
            EnemyController controller = (EnemyController) ship.ControlNode;

            if (controller.Faction != Faction.Collective)
                return;
            
            if(controller.ChangeState(controller.Profile.FindState(AIStateType.WarpIn)))
                controller.TargetingOrder = ((EnemyController)user.ControlNode).TargetingOrder;
        }
    }
}