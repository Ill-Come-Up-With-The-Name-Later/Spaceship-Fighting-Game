using System.Collections.Generic;
using Godot;

/// <summary>
/// A ship's weapon system
/// </summary>
public partial class WeaponSystem : Node2D
{
    public Ship ParentShip;
    
    public List<List<Weapon>> Weapons;
    public List<Weapon> WeaponList;
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
        WeaponList = new List<Weapon>();
        ActiveWeaponTypeIndex = 0;

        foreach (Node child in GetChildren())
        {
            if (child is Weapon weapon)
            {
                WeaponList.Add(weapon);
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

    /// <summary>
    /// Gets the next available weapon of a type
    /// </summary>
    /// <param name="typeIndex">
    /// The index of the current weapon type
    /// </param>
    /// <returns>
    /// The next available weapon of the type
    /// </returns>
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

    /// <summary>
    /// Finds a weapon type's index
    /// </summary>
    /// <typeparam name="T">
    /// The type of weapon
    /// </typeparam>
    /// <returns>
    /// The index of the weapon type or -1
    /// if it is absent
    /// </returns>
    public int Find<T>() where T : Weapon
    {
        for(int i = 0; i < Weapons.Count; i++) {
            if (Weapons[i][0].GetType() == typeof(T))
                return i;
        }
    
        return -1;
    }

    /// <summary>
    /// Finds the index of weapons of the same
    /// type as passed in
    /// </summary>
    /// <param name="weapon">
    /// A weapon
    /// </param>
    /// <returns>
    /// The index of weapons of the same type
    /// </returns>
    public int FindOfSameType(Weapon weapon)
    {
        for (int i = 0; i < Weapons.Count; i++)
        {
            if(Weapons[i][0].GetType() == weapon.GetType())
                return i;
        }

        return -1;
    }
    
    /// <summary>
    /// If the ship can fire on a position based
    /// on FOV
    /// </summary>
    /// <param name="targetGlobalPosition">
    /// The target position
    /// </param>
    /// <returns>
    /// If the ship can fire at the target position
    /// </returns>
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