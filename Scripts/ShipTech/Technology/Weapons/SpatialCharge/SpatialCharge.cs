using Godot;

public partial class SpatialCharge : Weapon
{
    public override void Fire(Vector2 targetPosition)
    {
        FireProjectile(targetPosition);
    }
}