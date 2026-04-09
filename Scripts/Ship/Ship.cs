using System.Collections.Generic;
using Godot;
using Godot.Collections;
using SpaceshipFightingGame.Scripts.Controllers;
using SpaceshipFightingGame.Scripts.ShipTech;

public partial class Ship : CharacterBody2D
{
	public Engine Engine;
	public Hull Hull;
	public FusionReactor FusionReactor;
	public DefenseGenerator DefenseSystem;
	public WarpDrive WarpDrive;
	
	public Hurtbox Hurtbox;
	
	public Eventbus Eventbus;

	public WeaponSystem Weapons;
	
	public bool CanMove;
	public bool CanTurn;
	
	public Controllers Controller;

	public Sprite2D Sprite;

	public Array<Node> Technologies;
	
	/// <summary>
	/// Amount of materials the ship has
	/// </summary>
	public double Materials 
	{
		get => Hull.CurrentMaterials; 
		set => Hull.CurrentMaterials = value;
	} 
	
	/// <summary>
	/// The ship's current power amount
	/// </summary>
	public double CurrentPower
	{ 
		get => FusionReactor.CurrentPower; 
		set => FusionReactor.CurrentPower = value; 
	}

	/// <summary>
	/// The ship's active weapon
	/// </summary>
	public Weapon ActiveWeapon
	{
		get => Weapons.ActiveWeapon;
		set => Weapons.ActiveWeapon = value;
	}

	public override void _EnterTree()
	{
		Controller = Controllers.Computer;
	}

	public override void _Ready()
	{
		AddToGroup("Ship");
		
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
		
		Hurtbox = GetNode<Hurtbox>("Hurtbox");
		if(Hurtbox is null)
			GD.PrintErr("Hurtbox not found");
		
		Sprite =  GetNode<Sprite2D>("Sprite");
		
		if(Sprite is null)
			GD.PrintErr("Sprite not found");

		Technologies = GetNode<Node2D>("Technology").GetChildren();

		CanMove = true;
		CanTurn = true;

		DefenseSystem.DefenseActive = true;
	}
	
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// Gets a technology node if it exists
	/// </summary>
	/// <typeparam name="T">
	/// The type of the technology
	/// </typeparam>
	/// <returns>
	/// The node of the technology or null if absent
	/// </returns>
	public T GetTechnology<T>() where T : Technology
	{
		foreach(Node child in Technologies)
			if(child is T technology)
				return technology;
		
		return null;
	}

	/// <summary>
	/// If the ship is in warp
	/// </summary>
	/// <returns>
	/// If the ship is in warp
	/// </returns>
	public bool InWarp()
	{
		return WarpDrive.Active;
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
	/// The amount of power the ship has
	/// </summary>
	/// <returns>
	/// The amount of power the ship has
	/// </returns>
	public double AvailablePower()
	{
		return FusionReactor.CurrentPower;
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

	/// <summary>
	/// The ship's health
	/// </summary>
	/// <returns>
	/// The ship's health
	/// </returns>
	public double Health()
	{
		return Hull.CurrentHealth;
	}
}
