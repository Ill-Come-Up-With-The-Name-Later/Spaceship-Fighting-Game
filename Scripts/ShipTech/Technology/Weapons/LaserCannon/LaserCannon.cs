using Godot;

public partial class LaserCannon : Weapon
{
    public override void Fire(Vector2 targetPosition)
    {
        FireProjectile(targetPosition);
    }
}