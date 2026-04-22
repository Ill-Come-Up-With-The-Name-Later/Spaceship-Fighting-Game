using Godot;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;

public partial class Controller : Node2D
{
    [Export]
    public Faction Faction;

    public bool Dead = false;
}