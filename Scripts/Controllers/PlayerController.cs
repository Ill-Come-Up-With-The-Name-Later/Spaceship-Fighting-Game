using Godot;

public partial class PlayerController : Node2D
{
	public Ship ParentShip;
	
	public override void _Ready()
	{
		ParentShip = GetParent() as Ship;

		if (ParentShip is null)
			GD.PrintErr("PlayerController: no parent ship");
		else
			GD.Print("PlayerController: player ship found");
	}
	
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("accelerate"))
		{
			ParentShip.Velocity = (Vector2.Up * (float)ParentShip.Engine.TrueMoveSpeed()).Rotated(ParentShip.GlobalRotation);
			GD.Print("Player is moving: " + ParentShip.Velocity);
		}

		if (Input.IsActionPressed("turn_left"))
		{
			ParentShip.GlobalRotation -= Mathf.DegToRad(1);
		}

		if (Input.IsActionPressed("turn_right"))
		{
			ParentShip.GlobalRotation += Mathf.DegToRad(1);
		}
	}
}
