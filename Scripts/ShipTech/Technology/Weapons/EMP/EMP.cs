using Godot;
using Godot.Collections;

public partial class EMP : Weapon
{
    [Export]
    public double HitEffectRadius;

    [Export]
    public double PowerDrainHit;

    public Timer ChargeTimer;

    public override void _Ready()
    {
        base._Ready();
        
        ChargeTimer = GetNode<Timer>("ChargeTimer");

        if (ChargeTimer is null)
            GD.PrintErr("No charge timer");

        ChargeTimer.Timeout += HitTargetsNear;
    }

    public override void _ExitTree()
    {
        ChargeTimer.Timeout -= HitTargetsNear;
    }

    public override void Fire(Vector2 targetPosition)
    {
        base.Fire(targetPosition);
        
        ChargeTimer.Start();
        GD.Print("EMP charge started");
    }

    public void HitTargetsNear()
    {
        Array<Node> nodes = GetTree().GetNodesInGroup("Ship");

        foreach (Node node in nodes)
        {
            if (node is Ship ship)
            {
                if (ship.GlobalPosition.DistanceTo(ParentShip.GlobalPosition) <= HitEffectRadius)
                {
                    if (ship != ParentShip)
                    {
                        if(ship.HasUpgrade<ElectromagneticInsulator>())
                            ship.FusionReactor.CurrentPower = Mathf.Max(0, ship.FusionReactor.CurrentPower - (PowerDrainHit / 2));
                        else
                            ship.FusionReactor.CurrentPower = Mathf.Max(0, ship.FusionReactor.CurrentPower - PowerDrainHit);
                        
                        ship.Hurtbox.ApplyDamage(ship, ParentShip, WeaponStats.Damage,
                            WeaponStats.ShieldDamage, WeaponStats.PenetratingDamage);
                        
                        if (ship.Cloaked())
                        {
                            CloakingDrive drive = ship.GetTechnology<CloakingDrive>();
                            if(drive is not null)
                                if(drive.Active)
                                    drive.Toggle();
                        }
                        
                        GD.Print("EMP Hit " + ship.GetName());
                        
                        // Todo: Halving distress beacon range
                    }
                }
            }
        }
    }
}