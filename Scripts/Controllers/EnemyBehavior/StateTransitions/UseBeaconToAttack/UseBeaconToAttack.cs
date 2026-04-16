public partial class UseBeaconToAttack : StateTransition
{
    public override bool ShouldTransition(EnemyController controller, double deltaTime)
    {
        return true;
    }
}