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
    public EnemySpawnSet SpawnSet;
    
    private RandomNumberGenerator generator;

    [Export]
    public float SpawnpointRadius;

    private GameManager gameManager;

    public override void _Ready()
    {
        generator = new RandomNumberGenerator();
        
        Spawnpoints = new Array<Marker2D>();
        
        SpawnTimer = GetNode<Timer>("SpawnTimer");
        
        if(SpawnTimer is null)
            GD.PrintErr("No Spawn Timer found");
        
        Eventbus = GetNode<Eventbus>("/root/Eventbus");
        
        if(Eventbus is null)
            GD.PrintErr("Eventbus not found");
        
        gameManager = GetNode<GameManager>("/root/GameManager");
        
        if(gameManager is null)
            GD.PrintErr("GameManager not found");
        
        SpawnTimer.WaitTime = generator.RandfRange(MinSpawnTime, MaxSpawnTime);
        SpawnTimer.Timeout += Spawn;
        SpawnTimer.Start();
        
        CallDeferred(nameof(Spawn));
    }

    /// <summary>
    /// Creates random spawns
    /// </summary>
    public void CreateSpawns(int spawns = 20)
    {
        Spawnpoints.Clear();
        
        foreach(Node node in GetChildren())
            node.QueueFree();
        
        for (int i = 0; i < spawns; i++)
        {
            Marker2D spawnpoint = new Marker2D();
            Vector2 spawnpointPos = gameManager.PlayerShip.GlobalPosition + new Vector2(
                generator.RandfRange(-SpawnpointRadius, SpawnpointRadius),
                generator.RandfRange(-SpawnpointRadius, SpawnpointRadius));
            
            spawnpoint.GlobalPosition = spawnpointPos;
            AddChild(spawnpoint);
            Spawnpoints.Add(spawnpoint);
            
            GD.Print("Spawnpoint created at " + spawnpoint.GlobalPosition);
        }
    }

    /// <summary>
    /// Spawns an enemy
    /// </summary>
    public void Spawn()
    {
        int spawns = generator.RandiRange(MinSpawns, MaxSpawns);
        CreateSpawns(spawns * 2);

        for (int i = 0; i < spawns; i++)
        {
            Marker2D spawnpoint = Spawnpoints[generator.RandiRange(0, Spawnpoints.Count - 1)];
            
            ItemSelector<PackedScene> selector = new ItemSelector<PackedScene>();

            foreach (PackedScene enemy in SpawnSet.EnemyScenes.Keys)
                selector.AddItem(enemy, SpawnSet.EnemyScenes[enemy]);
            
            Ship ship = selector.Roll().Instantiate<Ship>();
            ship.GlobalPosition = spawnpoint.GlobalPosition;
            GetTree().CurrentScene.AddChild(ship);
        }
        
        SpawnTimer.WaitTime = generator.RandfRange(MinSpawnTime, MaxSpawnTime);
        SpawnTimer.Start();
    }
}