using Godot;
using Godot.Collections;

namespace SpaceshipFightingGame.Scripts.Global;

/// <summary>
/// The enemy spawn manager. Must be a node in a scene.
/// </summary>
public partial class EnemySpawnManager : Node2D
{
    public Array<Marker2D> Spawnpoints;
    public Eventbus Eventbus;
    public Timer SpawnTimer;

    [Export]
    public int MinSpawns;
    
    [Export]
    public int MaxSpawns;

    [Export]
    public float MinSpawnTime;
    
    [Export]
    public float MaxSpawnTime;
    
    [Export]
    public Array<PackedScene> Enemies;
    
    private RandomNumberGenerator generator;

    public override void _Ready()
    {
        generator = new RandomNumberGenerator();
        
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
        
        SpawnTimer.WaitTime = generator.RandfRange(MinSpawnTime, MaxSpawnTime);
        SpawnTimer.Timeout += Spawn;
        SpawnTimer.Start();
    }

    /// <summary>
    /// Spawns an enemy
    /// </summary>
    public void Spawn()
    {
        int spawns = generator.RandiRange(MinSpawns, MaxSpawns + 1);

        for (int i = 0; i < spawns; i++)
        {
            Marker2D spawnpoint = Spawnpoints[generator.RandiRange(0, Spawnpoints.Count)];
            
            // Todo: Replace this with weighted selection
            Ship ship = (Ship)Enemies[generator.RandiRange(0, Enemies.Count)].Instantiate();
            ship.GlobalPosition = spawnpoint.GlobalPosition;

            Eventbus.EmitSignal("EnemySpawned");
        }
        
        SpawnTimer.WaitTime = generator.RandfRange(MinSpawnTime, MaxSpawnTime);
        SpawnTimer.Start();
    }
}