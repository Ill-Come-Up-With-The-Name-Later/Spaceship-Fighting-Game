using System.Collections.Generic;
using SpaceshipFightingGame.Scripts.ShipTech;

public partial class DroneController : EnemyController
{
    public Ship SpawningShip;
    
    public override void _Process(double delta)
    {
        if (!IsInstanceValid(ParentShip))
            ChangeState(Profile.InitialState);
        
        if (Dead || !IsInstanceValid(ParentShip))
            return;
        
        TimeInState += delta;
        CurrentState?.Process(this, delta);
        EvaluateTransitions(delta);
    }
    
    public override PriorityQueue<Ship, double> TargetPriorities()
    {
        FlushTargetingToBuffer();
        
        PriorityQueue<Ship, double> queue = new PriorityQueue<Ship, double>();

        foreach (Ship ship in Manager.Ships)
        {
            if (ship == ParentShip)
                continue;
            
            if(ship == SpawningShip)
                continue;

            if (ship.GlobalPosition.DistanceTo(ParentShip.GlobalPosition) > DetectionRange)
                continue;

            if (ship.InWarp())
                continue;

            // Prevent targeting cloaked ships without the upgrade
            if (ship.Cloaked() && !ParentShip.HasUpgrade<AntiCloakScanner>())
                continue;
            
            // Prevent ships of the same faction from targeting each other
            if(!ParentShip.CanTarget(ship))
                continue;

            if (ship is Drone drone)
            {
                if(drone.ParentShip == ParentShip)
                    continue;
                
                if (drone.ControlNode is DroneController otherController)
                {
                    if (otherController.SpawningShip == SpawningShip)
                        continue;
                }
            }
            
            double priority = 0;
            
            foreach (Technology technology in ship.AllTechnologies)
                priority += technology.Weights.StrengthWeight;

            priority += ship.GetMissingHealthPercentage() * 5;
            priority += ship.GetMissingShieldPercentage() * 2;
            
            queue.Enqueue(ship, -priority);
        }

        RepopulateFromBuffer(ref queue);
        return queue;
    }
}