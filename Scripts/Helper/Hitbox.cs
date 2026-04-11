using Godot;

/// <summary>
/// A hitbox, attached to projectiles to deal
/// damage to a ship
/// </summary>
public partial class Hitbox : Area2D
{
	public Ship ParentShip;
	public Projectile ParentProjectile;
	public CollisionShape2D Collider;
	public Eventbus Eventbus;
	
	public override void _Ready()
	{
		ParentProjectile = GetParent() as Projectile;
		
		if(ParentProjectile is null)
			GD.PrintErr("Parent projectile is null");

		ParentShip = ParentProjectile.ParentShip;
		
		Collider = GetNode<CollisionShape2D>("Collision");
		
		if(Collider is null)
			GD.PrintErr("Collider is null");
		
		Eventbus = GetNode<Eventbus>("/root/Eventbus");
		
		if(Eventbus is null)
			GD.PrintErr("Eventbus is null");

		AreaEntered += OnEntered;
	}

	public override void _ExitTree()
	{
		AreaEntered -= OnEntered;
	}

	/// <summary>
	/// Emits the damage signal when needed
	/// </summary>
	/// <param name="area">
	/// The other Area2D that entered this one
	/// </param>
	public void OnEntered(Area2D area)
	{
		if (area is Hurtbox hurtbox)
		{
			if (hurtbox.ParentShip == ParentProjectile.ParentShip)
				return;
			
			Eventbus.EmitSignal("ShipDamaged", hurtbox.ParentShip, ParentShip, 
				ParentProjectile.ParentWeaponStats.Damage, ParentProjectile.ParentWeaponStats.ShieldDamage,
				ParentProjectile.ParentWeaponStats.PenetratingDamage);

			Eventbus.EmitSignal("SimplifiedShipDamaged", hurtbox.ParentShip, ParentShip);
			
			ParentProjectile.QueueFree();
		}
	}
}
