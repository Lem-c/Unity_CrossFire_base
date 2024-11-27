using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    public Weapon weaponManifest;
    public int currentAmmo;
    public GameObject holder;
    public WeaponAudioManager audioManager;

    protected IWeaponState currentState;
    protected Animator weaponAnimator;

    protected float lastFireTime;
    protected bool isReloading;
    protected bool isFiring;
    protected bool hasDrawnWeapon = false;
    protected bool isDrawing = false;

    public static int displayAmmo;
    public static string displayWeapon;

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
        HandleWeaponInput();
        HandleAnimationInput();

        // UI update
        displayAmmo = currentAmmo;
        displayWeapon = weaponManifest.weaponName;
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

        if (currentAmmo >= 0)
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

    private void HandleAnimationInput()
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

            ShootBullet();
            // change last fire time
            lastFireTime = Time.time;
        }
    }

    protected virtual void ShootBullet()
    {
        Camera playerCamera = Camera.main;
        if (playerCamera != null)
        {
            Vector3 bulletSpawnPosition = playerCamera.transform.position + playerCamera.transform.forward;
            Quaternion bulletSpawnRotation = playerCamera.transform.rotation;

            if (weaponManifest.bulletPrefab == null)
            {
                Debug.LogError("No bullet prefab defined in the weapon manifest!");
                return;
            }

            // Instantiate the single bullet prefab
            GameObject bullet = Instantiate(weaponManifest.bulletPrefab, bulletSpawnPosition, bulletSpawnRotation);
            if (bullet != null)
            {
                // Modify bullet behavior based on weapon type
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
        }
        else
        {
            Debug.LogError("No player camera found! Unable to spawn bullet.");
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