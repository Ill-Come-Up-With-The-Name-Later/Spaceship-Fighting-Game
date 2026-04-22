using Godot;
using Godot.Collections;
using SpaceshipFightingGame.Scripts.Global;

public partial class EMP : Weapon
{
    [Export]
    public double HitEffectRadius;

    [Export]
    public double PowerDrainHit;

    [Export]
    public PackedScene BurstParticle;

    public Timer ChargeTimer;
    
    public GameManager GameManager;

    public override void _Ready()
    {
        base._Ready();
        
        ChargeTimer = GetNode<Timer>("ChargeTimer");

        if (ChargeTimer is null)
            GD.PrintErr("No charge timer");

        ChargeTimer.Timeout += HitTargetsNear;
        
        GameManager = GetNode<GameManager>("/root/GameManager");
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
        GpuParticles2D burst = BurstParticle.Instantiate<GpuParticles2D>();
        GetTree().CurrentScene.AddChild(burst);

        burst.GlobalPosition = GlobalPosition;
        burst.GlobalRotation = ParentShip.GlobalRotation;
        burst.Restart();
        burst.Emitting = true;
        burst.Finished += burst.QueueFree;

        Array<Ship> ships = new Array<Ship>(GameManager.Ships);
        Eventbus.EmitSignal("EmpFired", ParentShip);
        
        foreach (Ship ship in ships)
        {
            if (ship.GlobalPosition.DistanceTo(ParentShip.GlobalPosition) <= HitEffectRadius)
            {
                if (ship != ParentShip)
                {
                    if(ship.HasUpgrade<ElectromagneticInsulator>())
                        ship.FusionReactor.CurrentPower = Mathf.Max(0, ship.FusionReactor.CurrentPower - (PowerDrainHit / 2));
                    else
                        ship.FusionReactor.CurrentPower = Mathf.Max(0, ship.FusionReactor.CurrentPower - PowerDrainHit);
                        
                    Eventbus.EmitSignal("ShipDamaged", ship, ParentShip, 
                        WeaponStats.Damage, WeaponStats.ShieldDamage, WeaponStats.PenetratingDamage);
                        
                    if (ship.Cloaked())
                    {
                        CloakingDrive drive = ship.GetTechnology<CloakingDrive>();
                        if(drive is not null)
                            if(drive.Active)
                                drive.Toggle();
                    }

                    if (ship.HasAbility<DistressBeacon>())
                    {
                        DistressBeacon beacon = ship.GetAbility<DistressBeacon>();
                        beacon.Range /= 2;
                    }

                    if (ship.HasPart<CollectiveLinkNode>())
                    {
                        CollectiveLinkNode link = ship.GetPart<CollectiveLinkNode>();
                        link.Range *= 0.8;
                        link.DrainRate *= 0.75;
                    }
                        
                    GD.Print("EMP Hit " + ship.GetName());
                    Eventbus.EmitSignal("EmpHitShip", ship, ParentShip);
                }
            }
        }
    }
}