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

		Eventbus.WeaponFired += DisableOnFiring;
		CooldownTimer.Timeout += Enable;
		Eventbus.SimplifiedShipDamaged += Disable;
	}

	public override void _ExitTree()
	{
		Eventbus.WeaponFired -= DisableOnFiring;
		CooldownTimer.Timeout -= Enable;
	}

	public override void _Process(double delta)
	{
		ConsumingPower = Active;
		
		if (ConsumingPower)
			if(!ConsumePowerIfAvailable(ParentShip, delta))
				Disable(ParentShip, ParentShip);

		if (Active)
		{
			counter += delta;
			var selfMod = ParentShip.Sprite.SelfModulate;
			selfMod.A = (float)(0.1f * Mathf.Sin(2f * Mathf.Pi * counter)) + 0.2f;
			ParentShip.Sprite.SelfModulate = selfMod;
		}
	}

	public bool ConsumePowerIfAvailable(Ship ship, double delta)
	{
		if (CurrentConsumption * delta > ship.CurrentPower)
			return false;
        
		ship.FusionReactor.CurrentPower = Mathf.Max(0, ship.CurrentPower - CurrentConsumption * delta);
		return true;
	}

	public void Enable()
	{
		CanToggle = true;
	}

	public void DisableOnFiring(Vector2 pos, Ship ship, Weapon weapon)
	{
		if (Active && ship == ParentShip)
		{
			Disable(ship, ship);
		}
	}

	public void Disable(Ship ship, Ship damager)
	{
		if (Active && ship == ParentShip)
		{
			GD.Print("Cloak has been disabled");
			
			Active = false;
			var selfMod = ParentShip.Sprite.SelfModulate;
			selfMod.A = 1;
			ParentShip.Sprite.SelfModulate = selfMod;
			CooldownTimer.Start();
			CanToggle = false;
			ConsumingPower = false;

			Eventbus.EmitSignal("ShipDecloaked", ship);
		}
	}

	public void Toggle()
	{
		if (!CanToggle)
			return;

		if (ParentShip.HasShield())
			return;
		
		if(Active)
			Disable(ParentShip, null);
		else
		{
			Active = !Active;
			
			if (Active)
				Eventbus.EmitSignal("ShipCloaked", ParentShip);
		}
	}
}
