using System.Collections.Generic;
using SpaceshipFightingGame.Scripts.ShipTech;

public partial class HunterController : EnemyController
{
    public override PriorityQueue<Ship, double> TargetPriorities()
    {
        while (TargetingOrder.Count > 0)
        {
            TargetingOrder.TryDequeue(out Ship ship, out double priority);
            TargetingBuffer.Enqueue(ship, priority);
        }
        
        PriorityQueue<Ship, double> queue = new PriorityQueue<Ship, double>();

        foreach (Ship ship in Manager.Ships)
        {
            if (ship == ParentShip)
                continue;

            if (ship.GlobalPosition.DistanceTo(ParentShip.GlobalPosition) > DetectionRange)
                continue;

            if (ship.InWarp())
                continue;
            
            if (!ship.HasPart<TranswarpDrive>())
                continue;

            // Prevent targeting cloaked ships without the upgrade
            if (ship.Cloaked() && !ParentShip.HasUpgrade<AntiCloakScanner>())
                continue;
            
            // Prevent ships of the same faction from targeting each other
            if(!ParentShip.CanTarget(ship))
                continue;
            
            double priority = 0;
            
            foreach (Technology technology in ship.AllTechnologies)
                priority += technology.Weights.StrengthWeight;

            priority += ship.GetMissingHealthPercentage() * 5;
            priority += ship.GetMissingShieldPercentage() * 2;
            
            queue.Enqueue(ship, -priority);
        }
        
        while (TargetingBuffer.Count > 0)
        {
            TargetingBuffer.TryDequeue(out Ship ship, out double priority);
            TargetingOrder.Enqueue(ship, priority);
        }

        return queue;
    }
}