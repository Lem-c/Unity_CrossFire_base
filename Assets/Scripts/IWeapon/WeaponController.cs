using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    public Weapon weaponManifest;
    public int currentAmmo;
    public GameObject holder;
    public WeaponAudioManager audioManager;

    protected IWeaponState currentState;
    protected Animator weaponAnimator;

    // Muzzle flash
    public GameObject muzzleFlashPrefab;
    public Transform muzzlePosition;
    public float startSzie = 0.5f;

    protected float lastFireTime;
    public bool isReloading { get; protected set; }
    public bool isFiring { get; protected set; }
    protected bool hasDrawnWeapon = false;
    protected bool isDrawing = false;

    public static string displayAmmo;
    public static string displayWeapon;
    public static Sprite currentWeaponSprite;

    // Public method to set Animator integer parameter
    public void SetAnimatorInteger(string parameterName, int value)
    {
        if (weaponAnimator != null)
        {
            weaponAnimator.SetInteger(parameterName, value);
        }
    }

    protected virtual void Start()
    {
        InitializeWeapon();
    }

    protected virtual void FixedUpdate()
    {
        // UI update
        displayAmmo = currentAmmo.ToString();
        displayWeapon = weaponManifest.weaponName;
        currentWeaponSprite = weaponManifest.weaponImage;
    }

    public virtual void InitializeWeapon()
    {
        // Replay reload
        isReloading = false;
        weaponAnimator = GetComponent<Animator>();

        // Debug.Log($"Initializing weapon: {gameObject.name}, Animator: {weaponAnimator}");

        // Only assign animatorOverride if it exists
        if (weaponManifest.animatorOverride != null)
        {
            weaponAnimator.runtimeAnimatorController = weaponManifest.animatorOverride;
        }

        isDrawing = true;
        StartCoroutine(WaitForDrawAnimation());

        if (currentAmmo >= 0)   // equals zero seems counterintuitive, but for seek of knife class
        {
            SetState(new WeaponIdleState()); // Set initial state to Idle
        }
        else
        {
            SetState(new WeaponReloadState());
        }
    }

    public virtual void HandleWeaponInput()
    {
        if (!isReloading)
        {
            if (hasDrawnWeapon && !isDrawing && Input.GetKey(KeyCode.Mouse0) && Time.time >= lastFireTime + weaponManifest.fireRate)
            {
                if (!CanShoot()) { return; }
                HandleShoot();
            }
            else if (hasDrawnWeapon && !isDrawing && Input.GetKey(KeyCode.R) && currentAmmo < weaponManifest.maxAmmo)
            {
                HandleReload();
            }
            else
            {
                isFiring = false;
                SetState(new WeaponIdleState()); // No input / Walk
            }
        }

        // Auto-reload if ammo is empty
        if (currentAmmo == 0 && !isReloading)
        {
            HandleReload();
        }

        currentState.HandleState(this);
    }

    public void HandleAnimationInput()
    {
        if (Input.GetKey(KeyCode.Mouse0) && !isReloading)
        {
            weaponAnimator.SetBool("isFiring", true);
            weaponAnimator.SetInteger("Walk", -1);
        }
        else
        {
            weaponAnimator.SetBool("isFiring", false);
        }
    }

    public void SetState(IWeaponState newState)
    {
        currentState = newState;
    }

    public virtual void HandleShoot()
    {
        SetState(new WeaponShootState());
    }

    public virtual void Shoot()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;

            isFiring = true;

            // display muzzle flash
            if (muzzleFlashPrefab != null && muzzlePosition != null)
            {
                GameObject muzzleFlashInstance = Instantiate(muzzleFlashPrefab, muzzlePosition.position, muzzlePosition.rotation, muzzlePosition);

                ParticleSystem particleSystem = muzzleFlashInstance.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    // Access the Main module and set start size
                    var mainModule = particleSystem.main;
                    mainModule.startSize = startSzie;
                }

                muzzleFlashInstance.transform.localPosition = Vector3.zero; // Ensures it appears exactly at the muzzle position in local coordinates
                muzzleFlashInstance.transform.localRotation = Quaternion.identity; // Keeps the rotation correct relative to the muzzle
            }

            ShootBullet();

            // change last fire time
            lastFireTime = Time.time;
        }
    }

    public virtual void Shoot(Vector3 aimPoint)
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;

            isFiring = true;

            ShootBullet(aimPoint);
            // change last fire time
            lastFireTime = Time.time;
        }
    }

    protected virtual void ShootBullet()
    {
        if (holder.CompareTag("Player"))
        {
            ShootBulletFromCamera();
        }
        else
        {
            Debug.LogError("Holder is not tagged as Player. Use the overloaded method to pass an aimPoint.");
        }
    }

    protected virtual void ShootBullet(Vector3 aimPoint)
    {
        if (weaponManifest.bulletPrefab == null)
        {
            Debug.LogError("No bullet prefab defined in the weapon manifest!");
            return;
        }

        // Spawn position and direction for shooting
        Vector3 bulletSpawnPosition = transform.position;
        Quaternion bulletSpawnRotation = Quaternion.LookRotation((aimPoint).normalized);

        InstantiateAndConfigureBullet(bulletSpawnPosition, bulletSpawnRotation);
    }

    private void ShootBulletFromCamera()
    {
        if (weaponManifest.bulletPrefab == null)
        {
            Debug.LogError("No bullet prefab defined in the weapon manifest!");
            return;
        }

        Camera playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("No player camera found! Unable to spawn bullet.");
            return;
        }

        // Spawn position and rotation from the camera's perspective
        Vector3 bulletSpawnPosition = playerCamera.transform.position + playerCamera.transform.forward;
        Quaternion bulletSpawnRotation = playerCamera.transform.rotation;

        InstantiateAndConfigureBullet(bulletSpawnPosition, bulletSpawnRotation);
    }

    private void InstantiateAndConfigureBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(weaponManifest.bulletPrefab, position, rotation);

        if (bullet == null)
        {
            Debug.LogError("Failed to instantiate the bullet.");
            return;
        }

        // Configure bullet behavior based on weapon type
        switch (weaponManifest.weaponType)
        {
            case WeaponType.Pistol:
                PistolBullet pistolBullet = bullet.GetComponent<PistolBullet>();
                if (pistolBullet != null)
                {
                    pistolBullet.SetShooter(holder != null ? holder : gameObject);
                }
                break;

            case WeaponType.Rifle:
                RifleBullet rifleBullet = bullet.GetComponent<RifleBullet>();
                if (rifleBullet != null)
                {
                    rifleBullet.SetShooter(holder != null ? holder : gameObject);
                }
                break;

            default:
                Debug.LogError($"Unsupported weapon type: {weaponManifest.weaponType}");
                break;
        }
    }


    public virtual void HandleReload()
    {
        SetState(new WeaponReloadState());
    }

    public virtual void Reload()
    {
        if (!isReloading && !isDrawing)
        {
            isReloading = true;
            weaponAnimator.SetTrigger("Reload");
            // Coroutine to simulate reload delay
            StartCoroutine(ReloadCoroutine());
        }
    }

    public bool IsFiring() { return isFiring; }

    public virtual bool CanShoot()
    {
        return weaponManifest.weaponType != WeaponType.Knife;
    }

    protected virtual IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(weaponManifest.reloadTime);
        currentAmmo = weaponManifest.maxAmmo;
        // Flag: reload finished
        isReloading = false;
    }

    protected virtual IEnumerator WaitForDrawAnimation()
    {
        yield return new WaitForSeconds(weaponAnimator.GetCurrentAnimatorStateInfo(0).length);
        isDrawing = false;
        hasDrawnWeapon = true;
    }
}