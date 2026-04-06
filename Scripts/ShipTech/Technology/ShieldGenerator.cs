using Godot;

public partial class ShieldGenerator : DefenseGenerator
{
    public override void _Ready()
    {
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public override bool Regenerate(double delta)
    {
        if (CurrentDefenseHealth < DefenseGeneratorStats.DefenseHealth)
        {
            CurrentDefenseHealth = Mathf.Max(DefenseGeneratorStats.DefenseHealth,
                CurrentDefenseHealth + (RegenerationRate.PerSecondRate * delta));
            
            return true;
        }
        
        return false;
    }
}