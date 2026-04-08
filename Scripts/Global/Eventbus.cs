using Godot;

public partial class Eventbus : Node
{
    [Signal]
    public delegate void MouseClickEventHandler(Vector2 position);
    
    [Signal]
    public delegate void ShipDamagedEventHandler(Ship damaged, Ship damager,
        double damage, double shieldDamage, double penetrationDamage);
}
