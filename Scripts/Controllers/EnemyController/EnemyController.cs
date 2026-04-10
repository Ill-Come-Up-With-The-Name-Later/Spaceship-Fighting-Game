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
    public double DetectionRange;
    
    public double TimeInState { get; set; }

    [Export]
    public AIProfile Profile;
    
    public AIState CurrentState { get; private set; }
    
    public Vector2 MovementTarget { get; set; }
	
    public override void _Ready()
    {
        ParentShip = GetParent() as Ship;
        Eventbus = GetNode<Eventbus>("/root/Eventbus");
        
        Manager = GetNode<GameManager>("/root/GameManager");
		
        if(Eventbus is null)
            GD.PrintErr("No Eventbus found");
		
        if (ParentShip is null)
            GD.PrintErr("Enemy Controller: no parent ship");
        
        GD.Print("Enemy Controller: ship found");
        MovementTarget = ParentShip.GlobalPosition;
        ParentShip.Controller = Controllers.Computer;
        
        if(Profile is null)
            GD.PrintErr("No AI found");
        
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

        Vector2 toTarget = MovementTarget - ParentShip.GlobalPosition;

        if (toTarget.Length() < 5f)
        {
            ParentShip.Velocity = Vector2.Zero;
            return;
        }

        float targetAngle = toTarget.Angle();
        float maxTurn = (float)ParentShip.Engine.EngineStats.TurningRadius * (float)delta;

        if (ParentShip.InWarp())
            maxTurn *= (float)ParentShip.WarpDrive.WarpDriveStats.TurnRate;
        
        ParentShip.Rotation = Mathf.RotateToward(ParentShip.Rotation, targetAngle, maxTurn);

        Vector2 forward = new Vector2(Mathf.Cos(ParentShip.Rotation), Mathf.Sin(ParentShip.Rotation));
        ParentShip.Velocity = forward * (float)ParentShip.Speed();

        ParentShip.MoveAndSlide();
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
            
            double priority = 0;
            
            foreach (Technology technology in ship.AllTechnologies)
                priority += technology.Weights.StrengthWeight;

            priority += ship.GetMissingHealthPercentage() * 5;
            priority += ship.GetMissingShieldPercentage() * 2;
            
            queue.Enqueue(ship, -priority);
        }

        return queue;
    }

    /// <summary>
    /// Creates the enemy's weapon firing priorities
    /// </summary>
    /// <returns>
    /// The weapon firing priority for the enemy
    /// </returns>
    public PriorityQueue<Weapon, double> WeaponPriorities()
    {
        PriorityQueue<Weapon, double> queue = new PriorityQueue<Weapon, double>();
        
        foreach(Weapon weapon in ParentShip.GetWeapons())
        {
            if (!weapon.CanFire)
                continue;

            double priority = weapon.WeaponStats.Damage / weapon.WeaponStats.FiringCooldown;
            queue.Enqueue(weapon, -priority);
        }

        return queue;
    }
}