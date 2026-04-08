using Godot;
using SpaceshipFightingGame.Scripts.Controllers;

public partial class PlayerController : Node2D
{
	/// <summary>
	/// The parent ship of this node
	/// </summary>
	public Ship ParentShip;
	public Eventbus Eventbus;
	
	public override void _Ready()
	{
		ParentShip = GetParent() as Ship;
		Eventbus = GetNode<Eventbus>("/root/Eventbus");
		
		if(Eventbus is null)
			GD.PrintErr("No Eventbus found");
		
		if (ParentShip is null)
			GD.PrintErr("PlayerController: no parent ship");
		else
		{
			GD.Print("PlayerController: player ship found");
			ParentShip.Controller = Controllers.Player;

			Eventbus.MouseClick += TriggerAttack;
		}
	}

	public override void _ExitTree()
	{
		Eventbus.MouseClick -= TriggerAttack;
	}

	public override void _Process(double delta)
	{
		if (ParentShip.CanMove)
		{
			if (Input.IsActionPressed("accelerate"))
				ParentShip.Velocity =
					(Vector2.Up * (float)ParentShip.Engine.TrueMoveSpeed()).Rotated(ParentShip.GlobalRotation);
			else
				ParentShip.Velocity = Vector2.Zero;
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

		if (ParentShip.Velocity.Length() > 0)
			GD.Print(ParentShip.Velocity);
	}

	public override void _PhysicsProcess(double delta)
	{
		ParentShip.MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
			if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
				Eventbus.EmitSignal("MouseClick", GetGlobalMousePosition());
	}

	/// <summary>
	/// Attacks with the active weapon
	/// </summary>
	/// <param name="position">
	/// The position clicked by the player
	/// </param>
	public void TriggerAttack(Vector2 position)
	{
		GD.Print("Player clicked " + position);
		
		if (!CanFireAt(position))
			return;

		if (ParentShip.ActiveWeapon is not null)
		{
			if (!ParentShip.ActiveWeapon.CanFire)
				ParentShip.ActiveWeapon =
					ParentShip.Weapons.NextAvailableOfType(ParentShip.Weapons.ActiveWeaponTypeIndex);

			if (ParentShip.ActiveWeapon.CanFire)
				ParentShip.ActiveWeapon.Fire(position);
		}
	}
	
	public bool CanFireAt(Vector2 targetGlobalPosition)
	{
		Vector2 forward = -Transform.Y.Normalized().Rotated(ParentShip.Rotation);
		Vector2 toTarget = (targetGlobalPosition - GlobalPosition).Normalized();

		float dot = forward.Dot(toTarget);
		
		float halfFovRad = Mathf.DegToRad((float)ParentShip.Weapons.AttackFOV * 0.5f);
		float threshold = Mathf.Cos(halfFovRad);

		return dot >= threshold;
	}
}
