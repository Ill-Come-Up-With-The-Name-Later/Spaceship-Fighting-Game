using Godot;
using Godot.Collections;

public partial class EnemySpawnSet : Resource
{
    [Export]
    public Dictionary<PackedScene, double> EnemyScenes;
}