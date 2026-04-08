using Godot;

/// <summary>
/// A hurtbox attached to ships to take
/// damage from projectiles
/// </summary>
public partial class Hurtbox : Area2D
{
	public Ship ParentShip;
	public CollisionShape2D Collider;
	public Eventbus Eventbus;
	
	public override void _Ready()
	{
		ParentShip = GetParent() as Ship;
		
		if(ParentShip is null)
			GD.PrintErr("Parent ship is null");
		
		Collider = GetNode<CollisionShape2D>("Collision");
		
		if(Collider is null)
			GD.PrintErr("Collider is null");
		
		Eventbus = GetNode<Eventbus>("/root/Eventbus");
		
		if(Eventbus is null)
			GD.PrintErr("Eventbus is null");
		
		Eventbus.ShipDamaged += ApplyDamage;
	}

	/// <summary>
	/// Applies damage to a ship
	/// </summary>
	/// <param name="damager">
	/// The ship that shot the projectile damaging this ship
	/// </param>
	/// <param name="damage">
	/// The amount of damage to deal
	/// </param>
	/// <param name="shieldDamage">
	/// The amount of damage to deal to shields/armor
	/// </param>
	/// <param name="penetratingDamage">
	/// The amount of damage to deal through shields/armor
	/// </param>
	public void ApplyDamage(Ship damaged, Ship damager, double damage, double shieldDamage, double penetratingDamage)
	{
		if (damaged != ParentShip)
			return;

		if (damager == damaged)
			return;
		
		GD.Print("Applying damage to " + damaged);

		if (damaged.HasShield())
		{
			damaged.DamageShield(damage + shieldDamage);
			damaged.DamageHull(penetratingDamage * 0.5);
		}
		else
		{
			damaged.DamageHull(damage + penetratingDamage);
			damaged.DamageShield(damage * 0.1);
			damaged.DamageReactor(damage * 0.1);
			damaged.DamageEngine(damage * 0.2);
		}
	}
}
