using Godot;
using SpaceshipFightingGame.Scripts.Controllers;
using SpaceshipFightingGame.Scripts.Global;

/// <summary>
/// The controller for the player
/// </summary>
public partial class PlayerController : Controller
{
	/// <summary>
	/// The parent ship of this node
	/// </summary>
	public Ship ParentShip;
	public Eventbus Eventbus;
	
	public GameManager GameManager;

	private float currentTurnSpeed = 0;
	private float previousRotation;

	public Timer AttackTimer;

	[Export]
	public double AttackCooldown;

	public bool CanAttack;
	
	public override void _Ready()
	{
		ParentShip = GetParent() as Ship;
		Eventbus = GetNode<Eventbus>("/root/Eventbus");
		
		GameManager = GetNode<GameManager>("/root/GameManager");
		
		if(GameManager.PlayerShip is null)
			GameManager.PlayerShip = ParentShip;
		else
			GD.PrintErr("The player ship is already present");
		
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

		AttackTimer = GetNode<Timer>("AttackTimer");
		
		if(AttackTimer is null)
			GD.PrintErr("No attack timer");

		AttackTimer.WaitTime = AttackCooldown;
		AttackTimer.Timeout += SetCanAttack;
		
		CanAttack = true;
		
		previousRotation = ParentShip.GlobalRotation;;
	}

	public override void _ExitTree()
	{
		Eventbus.MouseClick -= TriggerAttack;
		AttackTimer.Timeout -= SetCanAttack;
	}

	public override void _Process(double delta)
	{
		if (ParentShip.CanMove)
		{
			if (Input.IsActionPressed("accelerate"))
			{
				float speed = (float)ParentShip.Engine.TrueMoveSpeed();
				
				if (ParentShip.Velocity.Length() == 0)
					ParentShip.Velocity =
						(Vector2.Up * speed).Rotated(ParentShip.GlobalRotation);
				else
					ParentShip.Velocity = ParentShip.Velocity.Normalized() * speed;
			}
			else
				ParentShip.Velocity = Vector2.Zero;
		}
		
		float turnInput = 0f;

		if (ParentShip.CanTurn)
		{
			if (Input.IsActionPressed("turn_left"))
				turnInput -= 1f;

			if (Input.IsActionPressed("turn_right"))
				turnInput += 1f;
		}

		float turnSpeed = (float)ParentShip.Engine.EngineStats.TurningRadius;

		if (ParentShip.WarpDrive.Active)
			turnSpeed *= (float)ParentShip.WarpDrive.WarpDriveStats.TurnRate;
		
		float desiredTurnSpeed = turnInput * Mathf.DegToRad(turnSpeed);

		currentTurnSpeed = Mathf.Lerp(
			currentTurnSpeed,
			desiredTurnSpeed,
			0.5f
		);

		ParentShip.GlobalRotation += currentTurnSpeed * (float)delta;

		float deltaRotation = ParentShip.GlobalRotation - previousRotation;
		ParentShip.Velocity = ParentShip.Velocity.Rotated(deltaRotation);
		
		previousRotation = ParentShip.GlobalRotation;

		ParentShip.ActiveWeapon = ParentShip.Weapons
			.NextAvailableOfType(ParentShip.Weapons.ActiveWeaponTypeIndex);
		
		ListenForCloaking();
		ListenForShieldToggle();
		ListenForWarpDrive();
		WeaponSelection();
	}

	public override void _PhysicsProcess(double delta)
	{
		ParentShip.MoveAndSlide();
	}

	public void SetCanAttack()
	{
		CanAttack = true;
	}

	public void ListenForWarpDrive()
	{
		if (Input.IsActionJustPressed("toggle_warp_drive"))
		{
			if(ParentShip.WarpDrive.WarpDriveState is not (WarpDriveState.Accelerating or WarpDriveState.Decelerating))
				ParentShip.WarpDrive.Toggle();
		}
	}
	
	public void ListenForShieldToggle()
	{
		if (Input.IsActionJustPressed("toggle_shield"))
		{
			ParentShip.DefenseSystem.Toggle();
		}
	}

	public void ListenForCloaking()
	{
		if (Input.IsActionJustPressed("toggle_cloak"))
		{
			CloakingDrive drive = ParentShip.GetTechnology<CloakingDrive>();

			if (drive is not null)
			{
				drive.Toggle();
			}
		}
	}

	public void WeaponSelection()
	{
		if (Input.IsActionJustPressed("select_weapon_0"))
			ParentShip.Weapons.ActiveWeaponTypeIndex = 0;
		else if (Input.IsActionJustPressed("select_weapon_1"))
			ParentShip.Weapons.ActiveWeaponTypeIndex = 1;
		else if (Input.IsActionJustPressed("select_weapon_2"))
			ParentShip.Weapons.ActiveWeaponTypeIndex = 2;
		else if (Input.IsActionJustPressed("select_weapon_3"))
			ParentShip.Weapons.ActiveWeaponTypeIndex = 3;
		else if (Input.IsActionJustPressed("select_weapon_4"))
			ParentShip.Weapons.ActiveWeaponTypeIndex = 4;
		else if (Input.IsActionJustPressed("select_weapon_5"))
			ParentShip.Weapons.ActiveWeaponTypeIndex = 5;
		else if (Input.IsActionJustPressed("select_weapon_6"))
			ParentShip.Weapons.ActiveWeaponTypeIndex = 6;
		else if (Input.IsActionJustPressed("select_weapon_7"))
			ParentShip.Weapons.ActiveWeaponTypeIndex = 7;
		else if (Input.IsActionJustPressed("select_weapon_8"))
			ParentShip.Weapons.ActiveWeaponTypeIndex = 8;
		else if (Input.IsActionJustPressed("select_weapon_9"))
			ParentShip.Weapons.ActiveWeaponTypeIndex = 9;
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
		//GD.Print("Player clicked " + position);

		if (!CanAttack)
			return;
		
		if (!ParentShip.Weapons.CanFireAt(position))
			return;

		if (ParentShip.ActiveWeapon is not null)
		{
			if (!ParentShip.ActiveWeapon.CanFire)
				ParentShip.ActiveWeapon =
					ParentShip.Weapons.NextAvailableOfType(ParentShip.Weapons.ActiveWeaponTypeIndex);

			if (ParentShip.ActiveWeapon.CanFire)
			{
				ParentShip.ActiveWeapon.Fire(position);
				CanAttack = false;
				AttackTimer.Start();
			}
		}
	}
}
