using Godot;

public partial class Ship : CharacterBody2D
{
	public Engine Engine;
	public Hull Hull;
	public FusionReactor FusionReactor;
	public DefenseGenerator DefenseSystem;
	public WarpDrive WarpDrive;
	
	public bool CanMove;
	public bool CanTurn;
	
	public override void _Ready()
	{
		Engine = GetNode<Engine>("Engine");
		
		if(Engine is null)
			GD.PrintErr("Engine not found");
		
		Hull = GetNode<Hull>("Hull");
		
		if(Hull is null)
			GD.PrintErr("Hull not found");
		
		FusionReactor = GetNode<FusionReactor>("FusionReactor");
		
		if(FusionReactor is null)
			GD.PrintErr("Fusion Reactor not found");
		
		DefenseSystem = GetNode<DefenseGenerator>("ShieldGenerator");
		
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
