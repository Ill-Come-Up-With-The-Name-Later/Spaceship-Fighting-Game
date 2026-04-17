using System.Collections.Generic;
using Godot;
using Godot.Collections;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;

public partial class CollectiveManager : Node2D
{
    public HashSet<Ship> CollectiveShips;
    public Eventbus Eventbus;

    public double MaterialPool;
    
    public override void _EnterTree()
    {
        CollectiveShips = new HashSet<Ship>();
    }

    public override void _Ready()
    {
        Eventbus = GetNode<Eventbus>("/root/Eventbus");
        
        if(Eventbus is null)
            GD.PrintErr("Eventbus not found");
        
        Eventbus.DistressBeaconUsed += RespondToDistressBeacon;
        Eventbus.ShipSpawned += ReceiveSpawn;
        Eventbus.ShipDestroyed += ShipDestroyed;
    }

    public override void _Process(double delta)
    {
        DistributeMaterial(delta);
    }

    /// <summary>
    /// Grants materials equal to 5% of a ship's hull's
    /// maximum health to the global collective material pool
    /// </summary>
    /// <param name="ship">
    /// The destroyed ship
    /// </param>
    /// <param name="killer">
    /// The ship that destroyed the other
    /// </param>
    public void ShipDestroyed(Ship ship, Ship killer)
    {
        if (killer is null)
            return;

        if (killer.HasPart<CollectiveLinkNode>())
        {
            // Temporary value
            MaterialPool += ship.Hull.GenericStats.Health * 0.05;
            GD.Print("Material Pool: " + MaterialPool);
        }
    }

    /// <summary>
    /// Distributes material from the global material pool
    /// </summary>
    /// <param name="delta">
    /// Delta time
    /// </param>
    public void DistributeMaterial(double delta)
    {
        foreach (Ship ship in CollectiveShips)
        {
            if (ship.Hull.GetMissingHealthPercentage() > 10)
            {
                if (ship.HasPart<CollectiveLinkNode>())
                {
                    CollectiveLinkNode node = ship.GetPart<CollectiveLinkNode>();
                    
                    if(node.NearbyCollectiveLinks().Count == 0)
                        continue;
                    
                    double missingRatio = ship.Hull.GetMissingHealthPercentage();
                    
                    double demandPerSecond =
                        ship.Hull.Regenerator.ConsumptionRate.PerSecondRate * missingRatio;

                    double maxDrawPerSecond = node.DrainRate;
                    double transferRate = Mathf.Min(Mathf.Min(demandPerSecond, maxDrawPerSecond), MaterialPool);
                    double transferred = transferRate * delta;

                    MaterialPool -= transferred;
                    ship.Hull.CurrentMaterials += transferred;
                }
            }
        }
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

    
    /// <summary>
    /// Controls response to distress beacons
    /// </summary>
    /// <param name="user">
    /// The ship activating the distress beacon
    /// </param>
    /// <param name="used">
    /// The distress beacon used
    /// </param>
    public void RespondToDistressBeacon(Ship user, DistressBeacon used)
    {
        if (!CollectiveShips.Contains(user) || user.ControlNode is not EnemyController)
            return;

        Array<Ship> nearby = NearbyShips(user.GlobalPosition, used.Range);

        foreach (Ship ship in nearby)
        {
            EnemyController controller = (EnemyController) ship.ControlNode;
            
            if(ship == user)
                continue;

            if (controller.Faction != Faction.Collective)
                return;

            controller.PursuitTarget = user;
            if(controller.ChangeState(controller.Profile.FindState(AIStateType.WarpIn)))
                controller.TargetingOrder = ((EnemyController)user.ControlNode).TargetingOrder;
        }
    }
}