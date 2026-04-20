using System;
using System.Collections.Generic;
using Godot;
using SpaceshipFightingGame.Scripts.Controllers;
using SpaceshipFightingGame.Scripts.Global;
using SpaceshipFightingGame.Scripts.ShipTech;

/// <summary>
/// The controller for enemies
/// </summary>
public partial class EnemyController : Controller
{
    /// <summary>
    /// The parent ship of this node
    /// </summary>
    public Ship ParentShip;
    public Eventbus Eventbus;
    
    public GameManager Manager;

    /// <summary>
    /// The range that the enemy can target
    /// enemies from
    /// </summary>
    [Export] 
    public double DetectionRange;

    /// <summary>
    /// The range the enemy can fire at
    /// other enemies from
    /// </summary>
    [Export]
    public double FiringRange;
    
    public double TimeInState { get; set; }

    /// <summary>
    /// A counter that can be freely used by states.
    /// Will reset when a state exits.
    /// </summary>
    public double ManipulableCounter { get; set; }

    /// <summary>
    /// The enemy's AI behaviors
    /// </summary>
    [Export]
    public AIProfile Profile;
    
    public AIState CurrentState { get; private set; }
    
    public Vector2 MovementTarget { get; set; }

    /// <summary>
    /// Time between attack volleys
    /// </summary>
    public Timer AttackTimer;
    
    /// <summary>
    /// Time between re-sequencing targets
    /// </summary>
    public Timer TargetingTimer;

    /// <summary>
    /// Time between firing each individual shot
    /// </summary>
    public Timer PerAttackTimer;

    public PriorityQueue<Ship, double> TargetingOrder;
    public PriorityQueue<Weapon, double> WeaponFiringOrder;

    private float currentTurnSpeed;

    public bool CanAttack;

    public Ship PursuitTarget;
    
    /// <summary>
    /// A normalized vector representing forwards
    /// </summary>
    public Vector2 Forward
    {
        get => new Vector2(Mathf.Cos(ParentShip.Rotation - Mathf.Pi / 2), 
            Mathf.Sin(ParentShip.Rotation - Mathf.Pi / 2)).Normalized(); 
    }
    
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
        
        AttackTimer = GetNode<Timer>("AttackTimer");
        TargetingTimer = GetNode<Timer>("TargetingTimer");
        PerAttackTimer = GetNode<Timer>("PerAttackTimer");
        
        TargetingTimer.WaitTime = Profile.TargetingDelay;
        AttackTimer.WaitTime = Profile.AttackDelay;
        PerAttackTimer.WaitTime = Profile.PerAttackDelay;

        TargetingTimer.Timeout += SetupTargeting;
        AttackTimer.Timeout += SetupWeapons;
        PerAttackTimer.Timeout += SetCanAttack;
        
        TargetingOrder = new PriorityQueue<Ship, double>();
        WeaponFiringOrder = new PriorityQueue<Weapon, double>();

        CanAttack = true;
        
        ChangeState(Profile.InitialState);
        
        TargetingTimer.Start();
        AttackTimer.Start();

        Eventbus.SimplifiedShipDamaged += OnHit;
        
        CallDeferred(nameof(SetupTargeting));
    }

    public override void _ExitTree()
    {
        TargetingTimer.Timeout -= SetupTargeting;
        AttackTimer.Timeout -= SetupWeapons;
        PerAttackTimer.Timeout -= SetCanAttack;
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

       Movement(delta);
    }

    public void OnHit(Ship ship, Ship damager)
    {
        if(ship == ParentShip)
            TargetingOrder.Enqueue(damager, -20);
    }

    /// <summary>
    /// The enemy's movement
    /// </summary>
    /// <param name="delta">
    /// Delta time
    /// </param>
    public void Movement(double delta)
    {
        Vector2 toTarget = MovementTarget - ParentShip.GlobalPosition;
        
        if (CloseToMoveTarget())
        {
            ParentShip.Velocity = Vector2.Zero;
            return;
        }

        float targetAngle = Vector2.Up.AngleTo(toTarget);
        float maxTurn = (float)ParentShip.Engine.EngineStats.TurningRadius;

        if (ParentShip.InWarp())
            maxTurn *= (float)ParentShip.WarpDrive.WarpDriveStats.TurnRate;
        
        float angleDiff = Mathf.Wrap(targetAngle - ParentShip.Rotation, -Mathf.Pi, Mathf.Pi);
        float desiredTurnSpeed = Mathf.Clamp(angleDiff, -maxTurn, maxTurn);

        currentTurnSpeed = Mathf.MoveToward(currentTurnSpeed, desiredTurnSpeed, maxTurn);
        
        if(ParentShip.CanTurn)
            ParentShip.Rotation += currentTurnSpeed * (float)delta;

        Vector2 forward = new Vector2(Mathf.Cos(ParentShip.Rotation - Mathf.Pi / 2),
            Mathf.Sin(ParentShip.Rotation - Mathf.Pi / 2));

        Vector2 toTargetDir = toTarget.Normalized();

        float alignment = forward.Dot(toTargetDir);
        alignment = Mathf.Clamp(alignment, 0.2f, 1f);

        float speed = (float)ParentShip.Speed();
        
        if(ParentShip.CanTurn) 
            speed *= alignment;
        
        if(!ParentShip.InWarp())
            ParentShip.Velocity = forward * speed;
        
        ParentShip.MoveAndSlide();
    }

    /// <summary>
    /// Sets the targeting for the enemy
    /// </summary>
    public void SetupTargeting()
    {
        TargetingOrder = TargetPriorities();
    }

    /// <summary>
    /// Sets the weapon firing order
    /// </summary>
    public void SetupWeapons()
    {
        WeaponFiringOrder = WeaponPriorities();
    }

    /// <summary>
    /// Allows the enemy to attack
    /// </summary>
    public void SetCanAttack()
    {
        CanAttack = true;
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
        GD.Print(ParentShip.Name + " changed from " + (oldState is null ? "null" :
            oldState.GetType()) + " to " + newState.GetType());
        
        CurrentState?.ExitState(this, newState);
        
        CurrentState = newState;
        TimeInState = 0;
        ManipulableCounter = 0;
        CurrentState.EnterState(this, oldState);
        return true;
    }

    /// <summary>
    /// Actually fires an attack
    /// </summary>
    /// <param name="weapon">
    /// The weapon being fired
    /// </param>
    /// <param name="target">
    /// The target ship
    /// </param>
    /// <returns>
    /// If the weapon was fired
    /// </returns>
    public bool FireAt(Weapon weapon, Ship target)
    {
        if (!CanAttack)
            return false;

        if (target.GlobalPosition.DistanceTo(ParentShip.GlobalPosition) > FiringRange)
            return false;

        if (!ParentShip.Weapons.CanFireAt(target.GlobalPosition))
            return false;
        
        weapon.Fire(target);
        
        PerAttackTimer.Start();
        CanAttack = false;
        return true;
    }

    /// <summary>
    /// If the enemy is close to its target
    /// position
    /// </summary>
    /// <returns>
    /// If the enemy is close to its target
    /// position
    /// </returns>
    public bool CloseToMoveTarget()
    {
        return GlobalPosition.DistanceTo(MovementTarget) <= 100;
    }

    /// <summary>
    /// Creates the enemy's targeting priorities
    /// </summary>
    /// <returns>
    /// The targeting priorities
    /// </returns>
    public virtual PriorityQueue<Ship, double> TargetPriorities()
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
        RandomNumberGenerator generator = new RandomNumberGenerator();
        PriorityQueue<Weapon, double> queue = new PriorityQueue<Weapon, double>();
        
        foreach(Weapon weapon in ParentShip.GetWeapons())
        {
            if (!weapon.CanFire)
                continue;

            double priority = ((weapon.WeaponStats.Damage + (3 * weapon.WeaponStats.PenetratingDamage))
                               / weapon.WeaponStats.FiringCooldown);
            queue.Enqueue(weapon, -priority);
        }

        return queue;
    }
}