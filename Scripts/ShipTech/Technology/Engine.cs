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

	/// <summary>
	/// The ship's actual movement speed
	/// </summary>
	/// <returns>
	/// The ship's actual movement speed
	/// </returns>
	public double TrueMoveSpeed()
	{
		return Mathf.Max(0.1, EngineStats.Speed * GetHealthPercentage());
	}
}
