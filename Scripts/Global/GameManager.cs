using System.Collections.Generic;
using Godot;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;

namespace SpaceshipFightingGame.Scripts.Global;

public partial class GameManager : Node
{
    public Eventbus Eventbus;
    
    public List<Ship> Ships = new List<Ship>();

    public Ship PlayerShip;

    public override void _Ready()
    {
        Eventbus = GetNode<Eventbus>("/root/Eventbus");

        Eventbus.ShipSpawned += ShipSpawned;
        Eventbus.ShipDestroyed += ShipDestroyed;
    }

    public override void _Process(double delta)
    {
        if (PlayerShip is null)
        {
            foreach (Ship ship in Ships)
            {
                if (ship.ControlNode.Faction == Faction.Player)
                {
                    PlayerShip = ship;
                    break;
                }
            }
        }
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
    /// Adds ship to spawned list
    /// </summary>
    /// <param name="ship">
    /// The spawned ship
    /// </param>
    public void ShipSpawned(Ship ship)
    {
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
        if(Ships.Contains(ship))
            Ships.Remove(ship);
    }
}