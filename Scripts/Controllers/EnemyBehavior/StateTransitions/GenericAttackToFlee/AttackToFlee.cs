public partial class AttackToFlee : StateTransition
{
	public override bool ShouldTransition(EnemyController controller, double deltaTime)
	{
		return controller.ParentShip.GetMissingHealthPercentage() >= 0.75 
		       || controller.ParentShip.Engine.GetMissingHealthPercentage() >= 0.66;
	}
}