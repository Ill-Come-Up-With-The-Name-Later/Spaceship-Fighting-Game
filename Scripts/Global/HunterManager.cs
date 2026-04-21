using System.Collections.Generic;
using Godot;
using Godot.Collections;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;

public partial class HunterManager : Node2D
{
    public HashSet<Ship> HunterShips;
    public Eventbus Eventbus;

    public override void _EnterTree()
    {
        HunterShips = new HashSet<Ship>();
    }

    public override void _Ready()
    {
        Eventbus = GetNode<Eventbus>("/root/Eventbus");
        
        if(Eventbus is null)
            GD.PrintErr("Eventbus not found");
        
        Eventbus.ShipSpawned += ReceiveSpawn;
        Eventbus.TranswarpExited += RespondToTranswarpExit;
    }
    
    /// <summary>
    /// Deals with ship spawning and hooking in
    /// ships to the hunter faction
    /// </summary>
    /// <param name="ship">
    /// The ship that has spawned
    /// </param>
    public void ReceiveSpawn(Ship ship)
    {
        if(ship.ControlNode.Faction != Faction.Hunter)
            return;
        
        HunterShips.Add(ship);
    }
    
    /// <summary>
    /// Gets hunter ships near a position
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
        
        foreach (Ship ship in HunterShips)
        {
            if(ship.GlobalPosition.DistanceTo(pos) <= distance)
                nearby.Add(ship);
        }
        
        return nearby;
    }
    
    /// <summary>
    /// Controls response to transwarp exits
    /// </summary>
    /// <param name="user">
    /// The ship exiting transwarp
    /// </param>
    public void RespondToTranswarpExit(Ship user)
    {
        // Temporary value
        Array<Ship> nearby = NearbyShips(user.GlobalPosition, 28000);

        foreach (Ship ship in nearby)
        {
            EnemyController controller = (EnemyController) ship.ControlNode;
            
            if(ship == user)
                continue;

            controller.PursuitTarget = user;
            controller.ChangeState(controller.Profile.FindState(AIStateType.WarpIn));
        }
    }
}