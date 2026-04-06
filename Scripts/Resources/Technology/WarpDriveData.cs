using Godot;

namespace SpaceshipFightingGame.Scripts.Resources.Technology;

public partial class WarpDriveData : Resource
{
    [Export]
    public double SpeedMultiplier;
    
    [Export]
    public double MaxDuration;
    
    [Export]
    public double AccelerationTime;
    
    [Export]
    public double DecelerationTime;

    [Export]
    public double TurnRate;
}