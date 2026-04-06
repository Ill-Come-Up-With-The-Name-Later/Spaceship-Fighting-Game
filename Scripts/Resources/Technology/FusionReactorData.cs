using Godot;

namespace SpaceshipFightingGame.Scripts.Resources.Technology;

public partial class FusionReactorData : Resource
{
    [Export]
    public double PowerCapacity;

    [Export] public double Fuel;
}