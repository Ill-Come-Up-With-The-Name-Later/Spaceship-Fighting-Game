using Godot;
using Godot.Collections;

public partial class PhotonTorpedo : Weapon
{
    [Export]
    public double InitialTargetingRadius;

    public override void Fire(Vector2 targetPosition)
    {
        Projectile projectile = FireProjectile(targetPosition);
        
        if (projectile is not null)
        {
            PhotonTorpedoProjectile torpedoProjectile = projectile as PhotonTorpedoProjectile;
            torpedoProjectile.Target = FindTarget(targetPosition);
            torpedoProjectile.fallbackPosition = targetPosition;
        }
    }

    public Ship FindTarget(Vector2 targetPosition)
    {
        Array<Node> nodes = GetTree().GetNodesInGroup("Ship");

        foreach (Node node in nodes)
        {
            if (node is Ship ship)
            {
                if (ship.GlobalPosition.DistanceTo(targetPosition) <= InitialTargetingRadius)
                {
                    if (ship != ParentShip)
                        return ship;
                }
            }
        }

        return null;
    }
}