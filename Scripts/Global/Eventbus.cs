using Godot;

public partial class Eventbus : Node
{
    [Signal]
    public delegate void MouseClickEventHandler(Vector2 position);

    [Signal]
    public delegate void WeaponFiredEventHandler(Vector2 position, Ship ship, Weapon weapon);
    
    [Signal]
    public delegate void ShipDamagedEventHandler(Ship damaged, Ship damager,
        double damage, double shieldDamage, double penetrationDamage);
    
    [Signal]
    public delegate void EmpFiredEventHandler(Ship user);
    
    [Signal]
    public delegate void EmpHitShipEventHandler(Ship hit, Ship user);

    [Signal]
    public delegate void SimplifiedShipDamagedEventHandler(Ship damaged);
    
    [Signal]
    public delegate void ShipEnterWarpEventHandler(Ship ship);
    
    [Signal]
    public delegate void ShipLeaveWarpEventHandler(Ship ship);
    
    [Signal]
    public delegate void ShipCloakedEventHandler(Ship ship);
    
    [Signal]
    public delegate void ShipDecloakedEventHandler(Ship ship);
    
    [Signal]
    public delegate void ShipDestroyedEventHandler(Ship ship, Ship killer);

    [Signal]
    public delegate void AbilityUsedEventHandler(Ability ability, Ship user);
    
    [Signal]
    public delegate void DistressBeaconUsedEventHandler(Ship user);
    
    [Signal]
    public delegate void TranswarpExitedEventHandler(Ship user);
}
