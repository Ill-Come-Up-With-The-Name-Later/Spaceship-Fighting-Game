public partial class AttackToUseTransporter : StateTransition
{
	public override bool ShouldTransition(EnemyController controller, double deltaTime)
	{
		if (controller.ParentShip.GetWeaponsOfType<Transporter>().Count == 0)
			return false;

		if (controller.TargetingOrder.Peek() is null)
			return false;

		if (controller.TargetingOrder.Peek().DefenseSystem.DefenseActive)
			return false;

		return controller.ParentShip.GetWeaponsOfType<Transporter>()[0].CanFire;
	}
}
