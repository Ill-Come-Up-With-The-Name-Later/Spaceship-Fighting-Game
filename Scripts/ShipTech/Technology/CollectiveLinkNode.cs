using Godot;
using Godot.Collections;
using SpaceshipFightingGame.Scripts.ShipTech;

public partial class CollectiveLinkNode : ShipPart
{
	[Export]
	public CollectiveNodeLinkRange LinkRange;
	
	private CollectiveManager collectiveManager;

	public double Range;
	public double DrainRate;
	
	public override void _Ready()
	{
		base._Ready();
		
		Range = LinkRange.LinkRange;
		DrainRate = LinkRange.MaxDrainRate;
		
		collectiveManager = GetNode<CollectiveManager>("/root/CollectiveManager");
		
		if(collectiveManager is null)
			GD.PrintErr("Collective Manager not found");
	}

	public override void _Process(double delta)
	{
	}

	public Array<Ship> NearbyCollectiveLinks()
	{
		Array<Ship> nearby = new Array<Ship>();
		
		foreach (Ship ship in collectiveManager.CollectiveShips)
		{
			if (ship == ParentShip)
				continue;
			
			if(ship.GlobalPosition.DistanceTo(ParentShip.GlobalPosition) <= Range)
				nearby.Add(ship);
		}
		
		return nearby;
	}
}
