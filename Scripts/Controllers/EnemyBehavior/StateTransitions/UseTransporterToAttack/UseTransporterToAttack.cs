public partial class UseTransporterToAttack : StateTransition
{
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        return true;
    }
}