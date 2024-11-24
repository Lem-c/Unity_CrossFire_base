using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    public Weapon weaponManifest;
    public int currentAmmo;
    public GameObject holder;

    protected IWeaponState currentState;
    protected Animator weaponAnimator;

    protected float lastFireTime;
    protected bool isReloading;
    protected bool hasDrawnWeapon = false;
    protected bool isDrawing = false;

    public static int displayAmmo;

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

    protected virtual void Update()
    {
        // HandleWeaponInput();
        displayAmmo = currentAmmo;
    }

    public virtual void InitializeWeapon()
    {
        // Replay reload
        isReloading = false;
        weaponAnimator = GetComponent<Animator>();
        // Only assign animatorOverride if it exists
        if (weaponManifest.animatorOverride != null)
        {
            weaponAnimator.runtimeAnimatorController = weaponManifest.animatorOverride;
        }

        // Draw sound and weapon
        WeaponAudioManager.Instance.PlayDrawSound();
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
            WeaponAudioManager.Instance.PlayFireSound();
            currentAmmo--;
            weaponAnimator.SetTrigger("Fire");
            ShootBullet();
            // change last fire time
            lastFireTime = Time.time;
        }
    }

    protected virtual void ShootBullet()
    {
        // Shooting logic: Instantiate bullet
        Camera playerCamera = Camera.main;
        if (playerCamera != null)
        {
            Vector3 bulletSpawnPosition = playerCamera.transform.position + playerCamera.transform.forward;
            Quaternion bulletSpawnRotation = playerCamera.transform.rotation;
            if (weaponManifest.bulletPrefab == null)
            {
                Debug.LogError("No bullet defined");
            }
            else
            {
                GameObject bullet = Instantiate(weaponManifest.bulletPrefab, bulletSpawnPosition, bulletSpawnRotation);
                PistolBullet instantiatedBullet = bullet.GetComponent<PistolBullet>();
                if (instantiatedBullet != null)
                {
                    instantiatedBullet.SetShooter(holder != null ? holder : gameObject); // Set the shooter to avoid self-hit
                }
                // Debug.Log("Bullet out!");
            }
        }
        else
        {
            Debug.Log("Assign bullet factory!");
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
            WeaponAudioManager.Instance.PlayReloadSound();
            isReloading = true;
            weaponAnimator.SetTrigger("Reload");
            // Coroutine to simulate reload delay
            StartCoroutine(ReloadCoroutine());
        }
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
