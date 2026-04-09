using Godot;

public partial class ShieldGenerator : DefenseGenerator
{
    public override void _Ready()
    {
        base._Ready();
        
        DefenseActive = true;
        Regenerating = true;
        ConsumingPower = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        ConsumingPower = DefenseActive;

        if (ConsumingPower) {
            if (ConsumePowerIfAvailable(ParentShip, delta))
            {
                if (Regenerating)
                    Regenerate(delta);
            }
            else
                DefenseActive = false;
        }
    }
    
    public new void Toggle()
    {
        DefenseActive = !DefenseActive;
    }

    public override bool Regenerate(double delta)
    {
        if (CurrentDefenseHealth < DefenseGeneratorStats.DefenseHealth)
        {
            CurrentDefenseHealth = Mathf.Min(DefenseGeneratorStats.DefenseHealth,
                CurrentDefenseHealth + (RegenerationRate.PerSecondRate * delta));

            return true;
        }

        return false;
    }
}