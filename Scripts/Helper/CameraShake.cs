using Godot;

namespace SpaceshipFightingGame.Scripts.Helper;

public partial class CameraShake : Camera2D
{
    public Eventbus Eventbus;

    public float ShakeStrength = 0;
    
    [Export]
    public float ShakeDecay;
    
    private RandomNumberGenerator generator;

    public Ship ParentShip;
    
    public override void _Ready()
    {
        Eventbus = GetNode<Eventbus>("/root/Eventbus");

        if(Eventbus is null)
            GD.PrintErr("Eventbus is null");

        Eventbus.ShipDamaged += ShakeCamera;
        
        generator = new RandomNumberGenerator();
        
        ParentShip = GetParent() as Ship;
    }

    public override void _Process(double delta)
    {
        float xOffset = generator.RandfRange(-ShakeStrength, ShakeStrength);
        float yOffset = generator.RandfRange(-ShakeStrength, ShakeStrength);
        
        Vector2 offset = new Vector2(xOffset, yOffset);
        
        if (offset.X != 0 || offset.Y != 0)
        {
            offset.X = Mathf.Lerp(offset.X, offset.X, 1);
            offset.Y = Mathf.Lerp(offset.Y, offset.Y, 1);
        }
        
        Offset += offset * (float)delta;
        ShakeStrength = Mathf.Max(0, ShakeStrength - ShakeDecay);
        //GD.Print("Shake Strength: " + ShakeStrength);
        
        if(ShakeStrength == 0) 
        {
            if(offset.X != 0 || offset.Y != 0)
                Offset = Vector2.Zero;
            
            if(GetParent() is not null) 
                GlobalPosition = ((Node2D)GetParent()).GlobalPosition;
        }
    }

    public void ShakeCamera(Ship damaged, Ship damager, double damage,
        double shieldDamage, double penetratingDamage)
    {
        if (damaged is null)
            return;

        if (damaged != ParentShip)
            return;
        
        ShakeStrength = (float)damage * 0.25f;
    }
}