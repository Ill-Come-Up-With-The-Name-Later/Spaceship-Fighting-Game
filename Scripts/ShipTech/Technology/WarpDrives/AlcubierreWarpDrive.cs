public partial class AlcubierreWarpDrive : WarpDrive
{
    public override void ActivateWarp()
    {
        base.ActivateWarp();
        ParentShip.CanTurn = false;
    }

    public override void DeactivateWarp()
    {
        base.DeactivateWarp();
        ParentShip.CanTurn = true;
    }
}