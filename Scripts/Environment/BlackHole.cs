using Godot;

public partial class BlackHole : Node
{
	public Area2D GravityWell;
	
	public override void _Ready()
	{
		GravityWell = GetNode<Area2D>("Gravity");
		
		if(GravityWell is null)
			GD.PrintErr("Black hole has no gravity well");
	}
	
	public override void _Process(double delta)
	{
	}
}
