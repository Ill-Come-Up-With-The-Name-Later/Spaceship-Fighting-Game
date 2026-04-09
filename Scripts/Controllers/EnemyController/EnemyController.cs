using Godot;

namespace SpaceshipFightingGame.Scripts.Controllers.EnemyController;

public partial class EnemyController : Node2D
{
    /// <summary>
    /// The parent ship of this node
    /// </summary>
    public Ship ParentShip;
    public Eventbus Eventbus;
    
    [Export]
    public Faction Faction;
	
    public override void _Ready()
    {
        ParentShip = GetParent() as Ship;
        Eventbus = GetNode<Eventbus>("/root/Eventbus");
		
        if(Eventbus is null)
            GD.PrintErr("No Eventbus found");
		
        if (ParentShip is null)
            GD.PrintErr("Enemy Controller: no parent ship");
        else
        {
            GD.Print("Enemy Controller: ship found");
            ParentShip.Controller = Controllers.Computer;
        }
    }
}