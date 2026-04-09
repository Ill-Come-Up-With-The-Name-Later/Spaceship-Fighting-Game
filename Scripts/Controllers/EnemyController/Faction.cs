namespace SpaceshipFightingGame.Scripts.Controllers.EnemyController;

/// <summary>
/// Enemy factions. Enemies within the same
/// faction will not attack each other unless
/// their faction is None.
/// </summary>
public enum Faction
{
    None,
    Collective,
    Hunter
}