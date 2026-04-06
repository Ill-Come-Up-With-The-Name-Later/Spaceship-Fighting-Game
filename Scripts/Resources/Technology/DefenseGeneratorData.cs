using Godot;

namespace SpaceshipFightingGame.Scripts.Resources.Technology;

public partial class DefenseGeneratorData : Resource
{
    [Export]
    public double DefenseHealth;
    
    [Export]
    public double DamageReductionPercent;
}