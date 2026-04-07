using Godot;

public partial class AblativeArmor : DefenseGenerator
{
    public override void _Ready()
    {
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        if(ConsumingPower && CurrentDefenseHealth < DefenseGeneratorStats.DefenseHealth)
            if(!DefenseActive && ConsumePowerIfAvailable(ParentShip, delta))
                Regenerate(delta);
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