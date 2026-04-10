using Godot;
using SpaceshipFightingGame.Scripts.Controllers.EnemyController;

namespace SpaceshipFightingGame.Scripts.Controllers;

public partial class Controller : Node2D
{
    [Export]
    public Faction Faction;
}