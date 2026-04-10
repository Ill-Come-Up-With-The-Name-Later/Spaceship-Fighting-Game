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

    [Export]
    public double detectionRange;
    
    public double TimeInState { get; set; }

    [Export]
    public AIProfile Profile;
    
    public AIState CurrentState { get; private set; }
	
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
        
        Profile.Initialize();
        ChangeState(Profile.InitialState);
    }

    public override void _Process(double delta)
    {
        TimeInState += delta;
        CurrentState?.Process(this, delta);
        EvaluateTransitions(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        CurrentState?.PhysicsProcess(this, delta);
    }

    /// <summary>
    /// Evaluates state transition conditions and
    /// triggers a state transition when needed
    /// </summary>
    /// <param name="delta">
    /// Delta time
    /// </param>
    public void EvaluateTransitions(double delta)
    {
        foreach (StateTransition transition in Profile.Transitions)
        {
            if(transition.From != CurrentState)
                continue;
            
            if (transition.ShouldTransition(this, delta))
            {
                ChangeState(transition.To);
                break;
            }
        }
    }

    /// <summary>
    /// Changes the enemy's state
    /// </summary>
    /// <param name="newState">
    /// The next state to change to
    /// </param>
    /// <param name="forceChange">
    /// If the state should be forced to change
    /// </param>
    /// <returns>
    /// If the state has changed
    /// </returns>
    public bool ChangeState(AIState newState, bool forceChange = false)
    {
        if (newState is null)
            return false;
        
        if (newState == CurrentState && !forceChange)
            return false;
        
        AIState oldState = CurrentState;
        CurrentState?.ExitState(this, newState);
        
        CurrentState = newState;
        TimeInState = 0;
        CurrentState.EnterState(this, oldState);
        return true;
    }

    /// <summary>
    /// Creates the enemy's targeting priorities
    /// </summary>
    /// <returns>
    /// The targeting priorities
    /// </returns>
    public PriorityQueue<Ship, double> TargetPriorities()
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
            
            foreach (Technology technology in ship.AllTechnologies)
                priority += technology.Weights.StrengthWeight;

            priority += ship.GetMissingHealthPercentage() * 5;
            priority += ship.GetMissingShieldPercentage() * 2;
            
            queue.Enqueue(ship, -priority);
        }

        return queue;
    }
}