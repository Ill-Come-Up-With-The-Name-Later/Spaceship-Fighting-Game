using Godot;

public partial class PlayerController : Node2D
{
	public CharacterBody2D parentShip;
	
	public override void _Ready()
	{
		parentShip = GetParent() as CharacterBody2D;

		if (parentShip is null)
			GD.PrintErr("PlayerController: no parent ship");
		else
			GD.Print("PlayerController: player ship found");
	}
	
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("accelerate"))
		{
			parentShip.Velocity = (Vector2.Up * 5).Rotated(parentShip.GlobalRotation);
			GD.Print("Player is moving: " + parentShip.Velocity);
		}

		if (Input.IsActionPressed("turn_left"))
		{
			parentShip.GlobalRotation -= Mathf.DegToRad(1);
		}

		if (Input.IsActionPressed("turn_right"))
		{
			parentShip.GlobalRotation += Mathf.DegToRad(1);
		}
	}
}
