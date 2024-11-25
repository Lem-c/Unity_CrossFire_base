using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;

    // Use an enum for weapon types to ensure consistency
    public WeaponType weaponType;

    public int maxAmmo;
    public float fireRate;
    public float reloadTime;

    // Single bullet prefab for all weapon types
    public GameObject bulletPrefab;

    public AnimatorOverrideController animatorOverride; // Animator controller for each weapon type
}

/// <summary>
/// Enum to define weapon types.
/// </summary>
public enum WeaponType
{
    Pistol,
    Rifle
}
