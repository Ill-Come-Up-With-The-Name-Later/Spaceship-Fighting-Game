using Godot;

public partial class AttackToFlee : StateTransition
{
	public override bool ShouldTransition(EnemyController controller, double deltaTime)
	{
		return controller.ParentShip.Hull.GetMissingHealthPercentage() >= 0.85 
		       && !controller.ParentShip.HasShield();
	}
}