using Godot;

public partial class PlasmaCannon : Weapon
{
    public override void Fire(Vector2 targetPosition)
    {
        FireProjectile(targetPosition);
    }
}