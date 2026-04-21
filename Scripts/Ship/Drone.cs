using Godot;

public partial class Drone : Ship
{
    public Ship ParentShip;

    public Timer LifespanTimer;

    [Export]
    public double Lifespan;
    
    public DroneMaterializer ParentMaterializer;

    public override void _Ready()
    {
        base._Ready();
        
        LifespanTimer = GetNode<Timer>("LifespanTimer");
        
        if(LifespanTimer is null)
            GD.Print("Drone lifespan timer not found");
        
        LifespanTimer.WaitTime = Lifespan;
        LifespanTimer.Timeout += QueueFree;
        LifespanTimer.Start();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        LifespanTimer.Timeout -= QueueFree;
        
        if(IsInstanceValid(ParentMaterializer))
            if(ParentMaterializer != null)
                ParentMaterializer.DroneActive = false;
    }
}