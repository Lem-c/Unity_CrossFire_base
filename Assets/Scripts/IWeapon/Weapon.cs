using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public int maxAmmo;
    public float fireRate;
    public float reloadTime;
    public GameObject bulletPrefab;
    public AnimatorOverrideController animatorOverride; // Animator controller for each weapon type
}
