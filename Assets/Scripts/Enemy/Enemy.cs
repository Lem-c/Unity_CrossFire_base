using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : WeaponController
{
    public EnemyState enemyState;
    public Transform playerTransform; // Reference to the player's transform
    public float detectionRange = 1000f; // Range within which the enemy can detect the player
    public float rotationSpeed = 5f; // Speed at which the enemy rotates toward the player

    protected override void Start()
    {
        // Initialize the enemy's player state
        enemyState = GetComponent<EnemyState>();
        if (enemyState == null)
        {
            enemyState = gameObject.AddComponent<EnemyState>();
        }

        // Assign initial ammo if not set
        if (currentAmmo == 0)
        {
            currentAmmo = weaponManifest.maxAmmo;
        }

        // Override the initialization to skip animations
        InitializeWeapon();
    }

    public override void InitializeWeapon()
    {
        isReloading = false;
        // Skip animator initialization and drawing animations
        hasDrawnWeapon = true; // Assume weapon is already drawn
        isDrawing = false;

        if (currentAmmo > 0)
        {
            SetState(new WeaponIdleState());
        }
        else
        {
            HandleReload();
        }
    }

    protected override void Update()
    {
        // HandleEnemyBehavior();
    }

    private void HandleEnemyBehavior()
    {
        if (playerTransform == null)
        {
            // Find the player in the scene
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Player not found in the scene!");
                return;
            }
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Rotate towards the player
            Vector3 direction = (playerTransform.position - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                // Rotate towards the player
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }

            // Shoot at the player
            HandleEnemyShooting();
        }

        // Update the current weapon state
        if (currentState != null)
        {
            currentState.HandleState(this);
        }
    }

    private void HandleEnemyShooting()
    {
        if (!isReloading && Time.time >= lastFireTime + weaponManifest.fireRate)
        {
            HandleShoot();
        }

        if (currentAmmo == 0 && !isReloading)
        {
            HandleReload();
        }
    }

    public override void HandleShoot()
    {
        SetState(new WeaponShootState());
        lastFireTime = Time.time;
    }

    public override void Shoot()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            ShootBullet();
        }
    }

    protected override void ShootBullet()
    {
        if (weaponManifest.bulletPrefab == null)
        {
            Debug.LogError("No bullet defined in weapon manifest!");
            return;
        }

        Vector3 bulletSpawnPosition = transform.position + transform.forward * 1f;
        Quaternion bulletSpawnRotation = Quaternion.LookRotation(transform.forward);

        GameObject bullet = Instantiate(weaponManifest.bulletPrefab, bulletSpawnPosition, bulletSpawnRotation);
        PistolBullet instantiatedBullet = bullet.GetComponent<PistolBullet>();
        if (instantiatedBullet != null)
        {
            instantiatedBullet.SetShooter(gameObject);
        }
    }

    public override void HandleReload()
    {
        SetState(new WeaponReloadState());
    }

    public override void Reload()
    {
        if (!isReloading)
        {
            isReloading = true;
            StartCoroutine(ReloadCoroutine());
        }
    }

    protected override IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(weaponManifest.reloadTime);
        currentAmmo = weaponManifest.maxAmmo;
        isReloading = false;
    }
}
