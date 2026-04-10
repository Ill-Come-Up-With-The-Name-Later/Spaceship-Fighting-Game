using System.Collections.Generic;
using Godot;
using SpaceshipFightingGame.Scripts.Global;
using SpaceshipFightingGame.Scripts.ShipTech;

namespace SpaceshipFightingGame.Scripts.Controllers.EnemyController;

public partial class EnemyController : Controller
{
    /// <summary>
    /// The parent ship of this node
    /// </summary>
    public Ship ParentShip;
    public Eventbus Eventbus;
    
    public GameManager Manager;
    
    [Export]
    public Faction Faction;

    public double detectionRange;
	
    public override void _Ready()
    {
        ParentShip = GetParent() as Ship;
        Eventbus = GetNode<Eventbus>("/root/Eventbus");
        
        Manager = GetNode<GameManager>("/root/GameManager");
		
        if(Eventbus is null)
            GD.PrintErr("No Eventbus found");
		
        if (ParentShip is null)
            GD.PrintErr("Enemy Controller: no parent ship");
        else
        {
            GD.Print("Enemy Controller: ship found");
            ParentShip.Controller = Controllers.Computer;
        }
    }

    /// <summary>
    /// Creates the enemy's targeting priorities
    /// </summary>
    /// <param name="detectionRange">
    /// The range of the enemy's detection
    /// </param>
    /// <returns>
    /// The targeting priorities
    /// </returns>
    public PriorityQueue<Ship, double> TargetPriorities(double detectionRange)
    {
        PriorityQueue<Ship, double> queue = new PriorityQueue<Ship, double>();

        foreach (Ship ship in Manager.Ships)
        {
            if (ship == ParentShip)
                continue;

            if (ship.GlobalPosition.DistanceTo(ParentShip.GlobalPosition) > detectionRange)
                continue;

            if (ship.InWarp())
                continue;

            // Prevent targeting cloaked ships without the upgrade
            if (ship.Cloaked() && !ParentShip.HasUpgrade<AntiCloakScanner>())
                continue;
            
            // Prevent ships of the same faction from targeting each other
            if(!ParentShip.CanTarget(ship))
                continue;
            
            double priority = 0;
            
            foreach (Technology technology in ship.GetTechnologies())
                priority += technology.Weights.StrengthWeight;

            priority += ship.GetMissingHealthPercentage() * 5;
            priority += ship.GetMissingShieldPercentage() * 2;
            
            priority = -priority;
            queue.Enqueue(ship, priority);
        }

        return queue;
    }
}