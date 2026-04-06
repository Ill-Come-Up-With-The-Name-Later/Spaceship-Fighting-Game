using SpaceshipFightingGame.Scripts.ShipTech;
using Godot;
using SpaceshipFightingGame.Scripts.Resources.Technology;

public partial class Engine : ShipPart
{
	[Export]
	public EngineData EngineStats;
	
	public override void _Ready()
	{
		// I don't know why this is needed, Rider seems to think it isn't
		// but this doesn't work without it.
		base._Ready();
	}

	public override void _Process(double delta)
	{
	}

	public double TrueMoveSpeed()
	{
		return EngineStats.Speed * GetHealthPercentage();
	}
}
