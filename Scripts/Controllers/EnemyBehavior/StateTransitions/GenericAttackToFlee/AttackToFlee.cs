public partial class AttackToFlee : StateTransition
{
	public override bool ShouldTransition(EnemyController controller, double deltaTime)
	{
		return !controller.ParentShip.HasShield() 
		       && controller.ParentShip.GetMissingHealthPercentage() >= 0.75;
	}
}