using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Components
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyState enemyState;
    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;


    // Movement and AI parameters
    public float fieldOfView = 90f;
    public float viewDistance = 50f;
    public float attackRange = 30f;
    public float turnSpeed = 10f;

    // Animation parameter hashes
    private int walkHash;
    private int isCrouchHash;
    private int isReloadHash;
    private int isFiringHash;

    // State variables
    private bool isCrouching;
    private bool isMoving;

    // Weapon
    public WeaponController weapon;
    protected float lastFireTime;

    // Destination for the enemy to move to
    [SerializeField]
    private Transform destination;
    [SerializeField]
    private Transform weaponMountPoint;

    private void Start()
    {
        // Initialize components
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found! Ensure the player GameObject is tagged as 'Player'.");
        }

        // Get animator parameter hashes
        walkHash = Animator.StringToHash("Walk");
        isCrouchHash = Animator.StringToHash("isCrouch");
        isReloadHash = Animator.StringToHash("isReload");
        isFiringHash = Animator.StringToHash("isFiring");

        // Initialize weapon
        if (weapon != null)
        {
            weapon.holder = gameObject; // Set the holder to the enemy
            weapon.InitializeWeapon();
        }
    }

    private void Update()
    {
        if (!enemyState.isAct)
            return; // Skip update if dead

        // AI behavior
        if (playerTransform != null && CanSeePlayer())
        {
            AimAtPlayer();
            // get aimed at player vector
            Vector3 aimPoint = (playerTransform.position - weaponMountPoint.position).normalized;

            if (IsPlayerInAttackRange())
            {
                if (aimPoint != null)
                {
                    AttackPlayer(aimPoint);
                }
            }
            else
            {
                MoveTo(playerTransform.position);
            }
        }
        else
        {
            Patrol();
        }

        UpdateAnimatorParameters();
    }

    // Methods to accept movement commands from AI system
    public void MoveTo(Vector3 targetPosition)
    {
        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(targetPosition);
            navMeshAgent.isStopped = false;
            isMoving = true;
        }
        else
        {
            Debug.Log("Not on nav mesh");
        }
    }

    public void StopMoving()
    {
        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
        }
        isMoving = false;
    }

    public void Crouch(bool crouch)
    {
        isCrouching = crouch;
    }

    public void Reload()
    {
        if (weapon != null && !weapon.isReloading)
        {
            weapon.Reload();
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < fieldOfView / 2f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer.normalized, out hit, viewDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void AimAtPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);

        // TODO: check whether aimed at player
    }

    private bool IsPlayerInAttackRange()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= attackRange;
    }

    private void AttackPlayer(Vector3 aimPoint)
    {
        StopMoving();

        if (weapon != null)
        {
            if (weapon.currentAmmo > 0 && !weapon.isReloading)
            {
                if (Time.time >= lastFireTime + weapon.weaponManifest.fireRate)
                {
                    weapon.Shoot(aimPoint);
                    weapon.audioManager.PlayFireSound();
                    // change last fire time
                    lastFireTime = Time.time;
                }
            }
            else if (!weapon.isReloading)
            {
                Reload();
            }
        }
    }

    private void Patrol()
    {
        if (destination != null)
        {
            MoveTo(destination.position);
        }
        else
        {
            StopMoving();
        }
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetBool(isCrouchHash, isCrouching);

        // Update walk animation based on movement
        float speed = navMeshAgent.velocity.magnitude;
        animator.SetInteger(walkHash, speed > 0.1f ? 1 : 0);

        // Update firing animation state from weapon
        animator.SetBool(isFiringHash, weapon != null && weapon.isFiring);

        // Update reloading animation state from weapon
        if (weapon != null && weapon.isReloading)
        {
            animator.SetTrigger(isReloadHash);
        }
    }
}
