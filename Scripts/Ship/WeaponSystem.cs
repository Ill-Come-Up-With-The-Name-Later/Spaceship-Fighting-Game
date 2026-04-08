using System.Collections.Generic;
using Godot;

/// <summary>
/// A ship's weapon system
/// </summary>
public partial class WeaponSystem : Node2D
{
    public List<Weapon> Weapons;
    public Weapon ActiveWeapon;

    [Export]
    public double AttackFOV;
    
    public override void _Ready()
    {
        Weapons = new List<Weapon>();

        foreach (Node child in GetChildren())
            if (child is Weapon weapon)
                Weapons.Add(weapon);
        
        GD.Print("Weapons found: " + Weapons.Count);

        if (Weapons.Count > 0)
            ActiveWeapon = Weapons[0];
    }

    public override void _Process(double delta)
    {
        
    }
}