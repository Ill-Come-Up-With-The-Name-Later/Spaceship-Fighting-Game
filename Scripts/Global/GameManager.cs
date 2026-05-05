using System.Collections.Generic;
using Godot;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;

namespace SpaceshipFightingGame.Scripts.Global;

public partial class GameManager : Node
{
    public Eventbus Eventbus;
    
    public HashSet<Ship> Ships = new HashSet<Ship>();

    public Ship PlayerShip;

    public override void _Ready()
    {
        Eventbus = GetNode<Eventbus>("/root/Eventbus");

        Eventbus.ShipSpawned += ShipSpawned;
        Eventbus.ShipDestroyed += ShipDestroyed;
    }

    /// <summary>
    /// Finds all ships of a faction
    /// </summary>
    /// <param name="faction">
    /// The faction
    /// </param>
    /// <returns>
    /// All ships in the faction
    /// </returns>
    public List<Ship> GetShipsOfFaction(Faction faction)
    {
        List<Ship> ships = new List<Ship>();
        
        foreach (Ship ship in Ships)
        {
            if (ship.ControlNode.Faction == faction)
                ships.Add(ship);
        }
        
        return ships;
    }

    /// <summary>
    /// Finds the first ship within a radius of a position
    /// </summary>
    /// <param name="position">
    /// The starting position
    /// </param>
    /// <param name="radius">
    /// The radius to search
    /// </param>
    /// <returns>
    /// The nearest ship or null
    /// </returns>
    public Ship GetNearbyShip(Vector2 position, float radius)
    {
        foreach (Ship ship in Ships)
        {
            if(ship.GlobalPosition.DistanceTo(position) <= radius)
                return ship;
        }
        
        return null;
    }

    /// <summary>
    /// Adds ship to spawned list
    /// </summary>
    /// <param name="ship">
    /// The spawned ship
    /// </param>
    public void ShipSpawned(Ship ship)
    {
        GD.Print("Ship spawned: " + ship.GetName() + " Active ships: " + Ships.Count);
        Ships.Add(ship);
    }

    /// <summary>
    /// Removes a ship from the list when destroyed
    /// </summary>
    /// <param name="ship">
    /// The destroyed ship
    /// </param>
    /// <param name="destroyer">
    /// The ship that destroyed the other
    /// </param>
    public void ShipDestroyed(Ship ship, Ship destroyer)
    {
        GD.Print("Ship destroyed: " + ship.GetName() + " Active ships: " + Ships.Count);
        if(Ships.Contains(ship))
            Ships.Remove(ship);
    }
}