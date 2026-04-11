using Godot;
using Godot.Collections;

namespace SpaceshipFightingGame.Scripts.Global;

public partial class EnemySpawnManager : Node2D
{
    public Array<Marker2D> Spawnpoints;
    public Eventbus Eventbus;
    public Timer SpawnTimer;

    [Export]
    public Array<PackedScene> Enemies;

    public override void _Ready()
    {
        Spawnpoints = new Array<Marker2D>();

        foreach (Node child in GetChildren())
        {
            if (child is Marker2D)
                Spawnpoints.Add((Marker2D)child);
        }
        
        if(Spawnpoints.Count == 0)
            GD.PrintErr("No spawn points found");
        
        SpawnTimer = GetNode<Timer>("SpawnTimer");
        
        if(SpawnTimer is null)
            GD.PrintErr("No Spawn Timer found");
        
        Eventbus = GetNode<Eventbus>("/root/Eventbus");
        
        if(Eventbus is null)
            GD.PrintErr("Eventbus not found");
    }
}