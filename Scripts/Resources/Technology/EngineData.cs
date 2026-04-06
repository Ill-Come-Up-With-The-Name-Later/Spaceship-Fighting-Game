using Godot;

namespace SpaceshipFightingGame.Scripts.Resources.Technology;

public partial class EngineData : Resource
{
    [Export]
    public double Speed;

    [Export] 
    public double TurningRadius;
}