using Godot;

public partial class AblativeArmor : DefenseGenerator
{
    public override void _Ready()
    {
        base._Ready();
        
        DefenseActive = true;
        Regenerating = false;
        ConsumingPower = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        if (CurrentHealth <= 0)
        {
            DefenseActive = false;
            CurrentDefenseHealth = 0;
        }
        
        if(CurrentDefenseHealth <= 0)
            DefenseActive = false;
        
        ConsumingPower = !DefenseActive;
        
        if(ConsumingPower && CurrentDefenseHealth < DefenseGeneratorStats.DefenseHealth)
            if(!DefenseActive && ConsumePowerIfAvailable(ParentShip, delta))
                Regenerate(delta);
        
        DefensePercentage = Mathf.Min(DefenseGeneratorStats.DamageReductionPercent, CurrentDefenseHealth / DefenseGeneratorStats.DefenseHealth);
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
                CurrentDefenseHealth = Mathf.Min(DefenseGeneratorStats.DefenseHealth,
                    CurrentDefenseHealth + (RegenerationRate.PerSecondRate * delta));

                return true;
            }
        }

        return false;
    }
}