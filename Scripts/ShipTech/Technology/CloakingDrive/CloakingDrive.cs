using Godot;
using SpaceshipFightingGame.Scripts.Resources.Technology;
using SpaceshipFightingGame.Scripts.ShipTech;

public partial class CloakingDrive : Technology, IPowerConsumer, IToggleable
{
	[Export]
	public PowerConsumptionData PowerConsumption { get; set; }

	[Export]
	public CloakingDriveStats Stats;

	public double CurrentConsumption { get; set; }

	public bool CanToggle { get; set; }

	public bool ConsumingPower { get; set; }

	public bool Active;

	public Timer CooldownTimer;

	public Ship ParentShip;
	
	public Eventbus Eventbus;

	private double counter;
	
	public override void _Ready()
	{
		CurrentConsumption = PowerConsumption.Amount;
		CanToggle = true;
		
		Active = false;
		ConsumingPower = false;

		counter = 0;
		
		CooldownTimer = GetNode<Timer>("CooldownTimer");
		
		if(CooldownTimer is null)
			GD.PrintErr("Cooldown Timer not found");

		CooldownTimer.WaitTime = Stats.Cooldown;
		
		Eventbus = GetNode<Eventbus>("/root/Eventbus");
		
		if(Eventbus is null)
			GD.PrintErr("Eventbus not found");
		
		ParentShip = GetParent().GetParent() as Ship;
		
		if(ParentShip is null)
			GD.PrintErr("Parent ship not found");

		Eventbus.WeaponFired += Disable;
		CooldownTimer.Timeout += Enable;
		Eventbus.SimplifiedShipDamaged += Disable;
	}

	public override void _ExitTree()
	{
		Eventbus.WeaponFired -= Disable;
		CooldownTimer.Timeout -= Enable;
		Eventbus.SimplifiedShipDamaged -= Disable;
	}

	public override void _Process(double delta)
	{
		ConsumingPower = Active;
		
		if (ConsumingPower)
			if(!ConsumePowerIfAvailable(ParentShip, delta))
				Disable(ParentShip);

		if (Active)
		{
			counter += delta;
			var selfMod = ParentShip.Sprite.SelfModulate;
			selfMod.A = (float)(0.1f * Mathf.Sin(4f * Mathf.Pi * counter)) + 0.25f;
			ParentShip.Sprite.SelfModulate = selfMod;
		}
	}

	public bool ConsumePowerIfAvailable(Ship ship, double delta)
	{
		if (CurrentConsumption > ship.CurrentPower)
			return false;
        
		ship.FusionReactor.CurrentPower -= CurrentConsumption * delta;

		return true;
	}

	public void Enable()
	{
		CanToggle = true;
	}

	public void Disable(Vector2 pos, Ship ship, Weapon weapon)
	{
		Disable(ship);
	}

	public void Disable(Ship ship)
	{
		Active = false;
		var selfMod = ParentShip.Sprite.SelfModulate;
		selfMod.A = 1;
		ParentShip.Sprite.SelfModulate = selfMod;
		CooldownTimer.Start();
		CanToggle = false;
	}

	public void Toggle()
	{
		if (!CanToggle)
			return;

		if (ParentShip.InWarp())
			return;

		if (ParentShip.HasShield())
			return;
		
		Active = !Active;
		GD.Print("Cloaking drive is active: " + Active);

		if (!Active)
		{
			Disable(ParentShip);
		}
	}
}
