using System.Collections.Generic;
using Godot;

/// <summary>
/// A ship's weapon system
/// </summary>
public partial class WeaponSystem : Node2D
{
    public Ship ParentShip;
    
    public List<List<Weapon>> Weapons;
    public Weapon ActiveWeapon;

    [Export]
    public double AttackFOV;

    public int ActiveWeaponTypeIndex;

    public override void _Ready()
    {
        ParentShip = GetParent() as Ship;
        
        if(ParentShip is null)
            GD.PrintErr("Parent ship is null");
        
        Weapons = new List<List<Weapon>>();
        ActiveWeaponTypeIndex = 0;

        foreach (Node child in GetChildren())
        {
            if (child is Weapon weapon)
            {
                bool added = false;
                
                foreach (List<Weapon> bucket in Weapons)
                {
                    if(child.GetType() == bucket[0].GetType())
                    {
                        bucket.Add(weapon);
                        added = true;
                        break;
                    }
                }

                if(!added)
                {
                    Weapons.Add(new List<Weapon>());
                    Weapons[Weapons.Count - 1].Add(weapon);
                    added = true;
                }
            }
        }
        
        GD.Print("Weapons: " + WeaponCount() + " Weapon Types: " + Weapons.Count);
        
        if (Weapons.Count > 0)
            ActiveWeapon = Weapons[ActiveWeaponTypeIndex][0];
    }

    /// <summary>
    /// The number of weapons on the ship
    /// </summary>
    /// <returns>
    /// The number of weapons on the ship
    /// </returns>
    public int WeaponCount()
    {
        int count = 0;
        
        foreach(List<Weapon> weapons in Weapons)
            count += weapons.Count;
        
        return count;
    }

    public Weapon NextAvailableOfType(int typeIndex)
    {
        if (typeIndex >= Weapons.Count)
            typeIndex = 0;
        
        foreach (Weapon weapon in Weapons[typeIndex])
        {
            if (weapon.CanFire)
                return weapon;
        }

        return Weapons[typeIndex][0];
    }
    
    public bool CanFireAt(Vector2 targetGlobalPosition)
    {
        Vector2 forward = -Transform.Y.Normalized().Rotated(ParentShip.Rotation);
        Vector2 toTarget = (targetGlobalPosition - GlobalPosition).Normalized();

        float dot = forward.Dot(toTarget);
		
        float halfFovRad = Mathf.DegToRad((float)ParentShip.Weapons.AttackFOV * 0.5f);
        float threshold = Mathf.Cos(halfFovRad);

        return dot >= threshold;
    }
}