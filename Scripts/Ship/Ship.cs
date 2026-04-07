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

		CanMove = true;
		CanTurn = true;
	}
	
	public override void _Process(double delta)
	{
	}
}
