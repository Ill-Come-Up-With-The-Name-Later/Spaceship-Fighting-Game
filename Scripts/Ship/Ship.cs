using Godot;
using SpaceshipFightingGame.Scripts.Controllers;

public partial class Ship : CharacterBody2D
{
	public Engine Engine;
	public Hull Hull;
	public FusionReactor FusionReactor;
	public DefenseGenerator DefenseSystem;
	public WarpDrive WarpDrive;
	
	public Eventbus Eventbus;

	public WeaponSystem Weapons;
	
	public bool CanMove;
	public bool CanTurn;
	
	public Controllers Controller;
	
	/// <summary>
	/// Amount of materials the ship has
	/// </summary>
	public double Materials { get => Hull.CurrentMaterials; 
		set => Hull.CurrentMaterials = value; } 
	
	/// <summary>
	/// The ship's current power amount
	/// </summary>
	public double CurrentPower { get => FusionReactor.CurrentPower; 
		set => FusionReactor.CurrentPower = value; }

	public override void _EnterTree()
	{
		Controller = Controllers.Computer;
	}

	public override void _Ready()
	{
		Eventbus = GetNode<Eventbus>("/root/Eventbus");
		
		if(Eventbus is null)
			GD.PrintErr("Eventbus not found");
		
		Engine = GetNode<Engine>("Engine");
		
		if(Engine is null)
			GD.PrintErr("Engine not found");
		
		Hull = GetNode<Hull>("Hull");
		
		if(Hull is null)
			GD.PrintErr("Hull not found");
		
		FusionReactor = GetNode<FusionReactor>("FusionReactor");
		
		if(FusionReactor is null)
			GD.PrintErr("Fusion Reactor not found");

		DefenseSystem = null;
		
		foreach(Node child in GetChildren())
			if(child is DefenseGenerator defenseGenerator)
				DefenseSystem = defenseGenerator;
		
		if(DefenseSystem is null)
			GD.PrintErr("Defense System not found");

		WarpDrive = null;
		
		foreach(Node child in GetChildren())
			if(child is WarpDrive warpDrive)
				WarpDrive = warpDrive;
		
		if(WarpDrive is null)
			GD.PrintErr("Warp Drive not found");

		Weapons = GetNode<WeaponSystem>("WeaponSystem");
		
		if(Weapons is null)
			GD.PrintErr("Weapons not found");

		CanMove = true;
		CanTurn = true;
	}

	/// <summary>
	/// Gets the ship's active weapon
	/// </summary>
	/// <returns>
	/// The ship's active weapon
	/// </returns>
	public Weapon ActiveWeapon()
	{
		return Weapons.ActiveWeapon;
	}

	/// <summary>
	/// If the ship has shields/armor
	/// </summary>
	/// <returns>
	/// If the ship has shields/armor
	/// </returns>
	public bool HasShield()
	{
		return DefenseSystem.DefenseActive;
	}

	/// <summary>
	/// Damage the hull
	/// </summary>
	/// <param name="damage">
	/// The damage to deal
	/// </param>
	public void DamageHull(double damage)
	{
		Hull.CurrentHealth = Mathf.Max(0, Hull.CurrentHealth - damage);
	}

	/// <summary>
	/// Damages the fusion reactor
	/// </summary>
	/// <param name="damage">
	/// The amount of damage to deal
	/// </param>
	public void DamageReactor(double damage)
	{
		FusionReactor.CurrentHealth = Mathf.Max(0, FusionReactor.CurrentHealth - damage);
	}

	/// <summary>
	/// Damages the shields/armor
	/// </summary>
	/// <param name="damage">
	/// The amount of damage to deal
	/// </param>
	public void DamageShield(double damage)
	{
		if (DefenseSystem.DefenseActive)
		{
			DefenseSystem.CurrentDefenseHealth = Mathf.Max(0, DefenseSystem.CurrentDefenseHealth - damage);
			DefenseSystem.CurrentHealth = Mathf.Max(0, DefenseSystem.CurrentHealth - (damage * 0.1));
		}
		else
			DefenseSystem.CurrentHealth = Mathf.Max(0, DefenseSystem.CurrentHealth - damage);
	}

	/// <summary>
	/// Damages the engine
	/// </summary>
	/// <param name="damage">
	/// The amount of damage to deal
	/// </param>
	public void DamageEngine(double damage)
	{
		Engine.CurrentHealth = Mathf.Max(0, Engine.CurrentHealth - damage);
	}
	
	public override void _Process(double delta)
	{
	}
}
