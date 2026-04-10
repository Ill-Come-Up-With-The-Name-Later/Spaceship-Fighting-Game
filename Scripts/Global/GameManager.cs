using System.Collections.Generic;
using Godot;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;

namespace SpaceshipFightingGame.Scripts.Global;

[GlobalClass]
public partial class GameManager : Node
{
    public List<Ship> Ships = new List<Ship>();

    public Ship PlayerShip;

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
}