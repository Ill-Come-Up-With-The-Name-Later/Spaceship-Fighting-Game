using Godot;

public partial class PlayerController : Node2D
{
	/// <summary>
	/// The parent ship of this node
	/// </summary>
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
		if (ParentShip.CanMove)
		{
			if (Input.IsActionPressed("accelerate"))
				ParentShip.Velocity =
					(Vector2.Up * (float)ParentShip.Engine.TrueMoveSpeed()).Rotated(ParentShip.GlobalRotation);
		}

		if (ParentShip.CanTurn)
		{
			if (Input.IsActionPressed("turn_left"))
			{
				if (ParentShip.WarpDrive.Active)
					ParentShip.GlobalRotation -= Mathf.DegToRad((float)ParentShip.Engine.EngineStats.TurningRadius
																* (float) ParentShip.WarpDrive.WarpDriveStats.TurnRate);
				else
					ParentShip.GlobalRotation -= Mathf.DegToRad((float)ParentShip.Engine.EngineStats.TurningRadius);
			}

			if (Input.IsActionPressed("turn_right"))
			{
				if (ParentShip.WarpDrive.Active)
					ParentShip.GlobalRotation += Mathf.DegToRad((float)ParentShip.Engine.EngineStats.TurningRadius
					                                            * (float) ParentShip.WarpDrive.WarpDriveStats.TurnRate);
				else
					ParentShip.GlobalRotation += Mathf.DegToRad((float)ParentShip.Engine.EngineStats.TurningRadius);
			}
		}
	}
}
