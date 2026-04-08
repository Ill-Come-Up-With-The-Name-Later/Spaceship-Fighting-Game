namespace SpaceshipFightingGame.Scripts.ShipTech;

/// <summary>
/// A part that is toggleable
/// </summary>
public interface IToggleable
{
    /// <summary>
    /// If this can be toggled or not
    /// </summary>
    public bool CanToggle { get; set; }

    /// <summary>
    /// Toggles the part
    /// </summary>
    public void Toggle();
}