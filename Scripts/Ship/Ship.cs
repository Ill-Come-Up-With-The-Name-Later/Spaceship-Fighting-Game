using System.Collections.Generic;
using Godot;
using Godot.Collections;
using SpaceshipFightingGame.Scripts.Controllers;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;
using SpaceshipFightingGame.Scripts.Global;
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
	public GameManager Manager;

	public WeaponSystem Weapons;
	
	public bool CanMove;
	public bool CanTurn;
	
	public Controllers Controller;
	public Controller ControlNode;

	public Sprite2D Sprite;

	public Array<Node> MiscTechnologies;
	
	public List<Technology> AllTechnologies { get; private set; }

	public Node2D TechnologyNode;
	public Node2D UpgradeNode;
	public Node2D AbilityNode;
	
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
		Controller = Controllers.None;
	}

	public override void _Ready()
	{
		Manager =  GetNode<GameManager>("/root/GameManager");
		AddToGroup("Ship");
		Manager.Ships.Add(this);
		
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
		
		TechnologyNode = GetNode<Node2D>("Technology");
		MiscTechnologies = TechnologyNode.GetChildren();
		
		ControlNode = GetNode<Controller>("ControlNode");
		
		if(ControlNode is null)
			GD.Print("Ship has no controller");

		UpgradeNode = GetNode<Node2D>("Upgrades");
		
		AbilityNode = GetNode<Node2D>("Abilities");

		CanMove = true;
		CanTurn = true;

		DefenseSystem.DefenseActive = true;

		AllTechnologies = GetAllTechnologies();
	}
	
	public override void _Process(double delta)
	{
		foreach (Upgrade upgrade in GetUpgrades())
			if (!upgrade.Applied.Contains(this) && upgrade.GetParent().GetParent() == this)
				upgrade.Apply(this);
	}

	public override void _ExitTree()
	{
		Manager.Ships.Remove(this);
	}

	/// <summary>
	/// Adds a weapon to the ship
	/// </summary>
	/// <param name="weapon">
	/// The weapon to add
	/// </param>
	public void AddWeapon(Weapon weapon)
	{
		Weapons.AddChild(weapon);
		AllTechnologies.Add(weapon);
	}

	/// <summary>
	/// Adds a technology to the ship
	/// </summary>
	/// <param name="technology">
	/// The technology to add
	/// </param>
	public void AddTechnology(Technology technology)
	{
		TechnologyNode.AddChild(technology);
		AllTechnologies.Add(technology);
		MiscTechnologies.Add(technology);
	}

	/// <summary>
	/// Adds an upgrade to the ship
	/// </summary>
	/// <param name="upgrade">
	/// The upgrade to add to the ship
	/// </param>
	public void AddUpgrade(Upgrade upgrade)
	{
		UpgradeNode.AddChild(upgrade);
		AllTechnologies.Add(upgrade);
	}

	/// <summary>
	/// Adds an ability to the ship
	/// </summary>
	/// <param name="ability">
	/// The ability to add to the ship
	/// </param>
	public void AddAbility(Technology ability)
	{
		// Todo: Implement Ability base class and change the parameter type
		AbilityNode.AddChild(ability);
		AllTechnologies.Add(ability);
	}

	/// <summary>
	/// Adds a ship part to the ship.
	/// Likely will end up only used for
	/// warp drives.
	/// </summary>
	/// <param name="part">
	/// The part to add to the ship
	/// </param>
	public void AddShipPart(ShipPart part)
	{
		AddChild(part);
		AllTechnologies.Add(part);
	}

	/// <summary>
	/// Removes a part from the ship
	/// </summary>
	/// <param name="part">
	/// The part to remove from the ship
	/// </param>
	public void RemovePart(ShipPart part)
	{
		RemoveChild(part);
		AllTechnologies.Remove(part);
	}
	
	/// <summary>
	/// Gets all technologies on the ship
	/// </summary>
	/// <returns>
	/// All technologies on the ship
	/// </returns>
	public List<Technology> GetAllTechnologies()
	{
		List<Technology> technologies = new();
		Stack<Node> nodes = new();
    
		nodes.Push(this);
		
		// DFS to the rescue
		while (nodes.Count > 0)
		{
			Node node = nodes.Pop();

			if (node is Technology technology)
				technologies.Add(technology);

			foreach (Node child in node.GetChildren())
				nodes.Push(child);
		}

		return technologies;
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
		foreach(Node child in MiscTechnologies)
			if(child is T technology)
				return technology;
		
		return null;
	}

	/// <summary>
	/// If the ship has an instance of a part
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// The type of part
	/// <returns>
	/// If the ship has an instance of a part
	/// </returns>
	public bool HasPart<T>() where T : ShipPart
	{
		foreach(Node child in GetChildren())
			if(child is T part)
				return true;
		
		return false;
	}

	/// <summary>
	/// Gets a part on the ship if it exists
	/// </summary>
	/// <typeparam name="T">
	/// The type of part
	/// </typeparam>
	/// <returns>
	///	The part if it exists
	/// </returns>
	public T GetPart<T>() where T : ShipPart
	{
		foreach(Node child in GetChildren())
			if(child is T part)
				return part;

		return null;
	}

	/// <summary>
	/// Gets an upgrade if the ship has it
	/// </summary>
	/// <typeparam name="T">
	/// The type of the upgrade
	/// </typeparam>
	/// <returns>
	/// The upgrade if the ship has it or null
	/// </returns>
	public T GetUpgrade<T>() where T : Upgrade
	{
		foreach(Node child in GetNode("Upgrades").GetChildren())
			if(child is T upgrade)
				return upgrade;
		
		return null;
	}

	/// <summary>
	/// If the ship has an upgrade
	/// </summary>
	/// <typeparam name="T">
	/// The type of upgrade
	/// </typeparam>
	/// <returns>
	/// If the ship has the upgrade
	/// </returns>
	public bool HasUpgrade<T>() where T : Upgrade
	{
		return GetUpgrade<T>() != null;
	}

	/// <summary>
	/// Gets all the upgrades on the ship
	/// </summary>
	/// <returns>
	/// All the upgrades on the ship
	/// </returns>
	public Array<Upgrade> GetUpgrades()
	{
		Array<Upgrade> upgrades = new Array<Upgrade>();
		
		foreach(Node child in UpgradeNode.GetChildren())
			if(child is Upgrade upgrade)
				upgrades.Add(upgrade);
		
		return upgrades;
	}
	
	/// <summary>
	/// Gets all weapons of a type
	/// </summary>
	/// <typeparam name="T">
	/// The type of weapon
	/// </typeparam>
	/// <returns>
	/// All weapons of a type
	/// </returns>
	public List<T> GetWeaponsOfType<T>() where T : Weapon
	{
		List<T> weapons = new List<T>();
		
		foreach (Node child in GetNode("Weapons").GetChildren())
		{ 
			if (child is T weapon)
				weapons.Add(weapon);		
		}
		
		return weapons;
	}

	/// <summary>
	/// If a ship can target another. Used by
	/// enemies
	/// </summary>
	/// <param name="other">
	/// The other ship
	/// </param>
	/// <returns>
	/// If this ship can target another
	/// </returns>
	public bool CanTarget(Ship other)
	{
		if (other == this)
			return false;

		if (ControlNode is EnemyController enemyController 
		    && other.ControlNode is EnemyController otherController)
		{
			if (enemyController.Faction == Faction.None)
				return true;
				
			return enemyController.Faction != otherController.Faction;
		}
		
		return true;
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
	/// If the ship is cloaked
	/// </summary>
	/// <returns>
	/// If the ship is cloaked
	/// </returns>
	public bool Cloaked()
	{
		CloakingDrive drive = GetTechnology<CloakingDrive>();
		
		if(drive is null)
			return false;
		
		return drive.Active;
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

	/// <summary>
	/// The percentage of health missing on the ship's hull
	/// </summary>
	/// <returns>
	/// The percentage of health missing on the ship's hull
	/// </returns>
	public double GetMissingHealthPercentage()
	{
		return Hull.GetMissingHealthPercentage();
	}

	/// <summary>
	/// The percentage of health missing on the ship's shield
	/// </summary>
	/// <returns>
	/// The percentage of health missing on the ship's shield
	/// </returns>
	public double GetMissingShieldPercentage()
	{
		return DefenseSystem.GetMissingDefenseHealthPercentage();
	}
}
