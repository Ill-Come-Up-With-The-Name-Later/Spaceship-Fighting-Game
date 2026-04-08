using Godot;

public partial class AblativeArmor : DefenseGenerator
{
    public double DefensePercentage;
    
    public override void _Ready()
    {
        base._Ready();

        DefensePercentage = DefenseGeneratorStats.DamageReductionPercent;
        
        DefenseActive = true;
        Regenerating = false;
        ConsumingPower = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        ConsumingPower = !DefenseActive;
        
        if(ConsumingPower && CurrentDefenseHealth < DefenseGeneratorStats.DefenseHealth)
            if(!DefenseActive && ConsumePowerIfAvailable(ParentShip, delta))
                Regenerate(delta);
        
        DefensePercentage = Mathf.Min(0.9, CurrentDefenseHealth / DefenseGeneratorStats.DefenseHealth);
    }

    public new void Toggle()
    {
        DefenseActive = !DefenseActive;
        Regenerating = !Regenerating;
        ConsumingPower = !ConsumingPower;
    }

    public override bool Regenerate(double delta)
    {
        if (!DefenseActive)
        {
            if (CurrentDefenseHealth < DefenseGeneratorStats.DefenseHealth)
            {
                CurrentDefenseHealth = Mathf.Max(DefenseGeneratorStats.DefenseHealth,
                    CurrentDefenseHealth + (RegenerationRate.PerSecondRate * delta));

                return true;
            }
        }

        return false;
    }
}