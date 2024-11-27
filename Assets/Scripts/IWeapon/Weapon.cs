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

    // Saved position info (x, y, z) for the weapon
    public Vector3 savedLocalPosition;
    // Saved rotation info for the weapon
    public Quaternion savedLocalRotation;
    public Vector3 savedLocalScale;

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
