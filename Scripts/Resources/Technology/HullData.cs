using Godot;

namespace SpaceshipFightingGame.Scripts.Resources.Technology;

public partial class HullData : Resource
{
    [Export]
    public double Materials;
    
    [Export]
    public double Scrap;
}