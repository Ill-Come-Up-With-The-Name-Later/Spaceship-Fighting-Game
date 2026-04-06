using Godot;
using SpaceshipFightingGame.Scripts.Resources;
using SpaceshipFightingGame.Scripts.Resources.Technology;
using SpaceshipFightingGame.Scripts.ShipTech;

public abstract partial class DefenseGenerator : ShipPart, IRegenerator
{
    [Export]
    public DefenseGeneratorData DefenseGeneratorStats;

    [Export]
    public RegenerationData RegenerationRate { get; set; }

    public bool Active { get; set; }

    public bool Regenerating { get; set; }

    public double CurrentDefenseHealth;

    public override void _Ready()
    {
        base._Ready();
        
        CurrentDefenseHealth = DefenseGeneratorStats.DefenseHealth;
        
        Active = true;
        Regenerating = true;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if(Regenerating)
            Regenerate(delta);
    }

    public abstract bool Regenerate(double delta);
}