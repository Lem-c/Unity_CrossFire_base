using UnityEngine;

public class KnifeWeaponController : WeaponController
{
    private Knife knifeManifest;
    private bool isAttacking;
    private bool isHeavy;
    private Vector3 lastAttackPosition;
    private float lastAttackRadius;

    // Time between consecutive attacks
    [SerializeField] private float attackCooldown = 1.0f;
    private float lastAttackTime;

    protected override void Start()
    {
        base.Start();

        // Ensure the weaponManifest is cast correctly to Knife
        knifeManifest = weaponManifest as Knife;
        if (knifeManifest == null)
        {
            Debug.LogError("Assigned weaponManifest is not of type Knife!");
        }

        isAttacking = false;
        isHeavy = false;

        // Initialize to allow immediate first attack
        lastAttackTime = -attackCooldown;
    }

    protected override void FixedUpdate()
    {
        // UI update
        displayAmmo = "";
        displayWeapon = weaponManifest.weaponName;
        currentWeaponSprite = weaponManifest.weaponImage;
    }

    public override void HandleWeaponInput()
    {
        if (!isReloading && hasDrawnWeapon && !isDrawing && Time.time >= lastAttackTime + attackCooldown)
        {
            if (Input.GetKey(KeyCode.Mouse0)) // Light attack
            {
                isAttacking = true;
                HandleLightAttack();
            }
            else if (Input.GetKey(KeyCode.Mouse1)) // Heavy attack
            {
                isAttacking = true;
                HandleHeavyAttack();
            }
        }

        base.HandleWeaponInput();
    }

    private void HandleLightAttack()
    {
        if (knifeManifest == null) return;

        isHeavy = false;
        weaponAnimator.SetTrigger("LightAttack");
        lastAttackTime = Time.time;

        // Duration to match animation
        Invoke("PerformOverlapAttack", 0.05f); // Timing for when the attack hits
        Invoke("EndAttack", 0.4f);
    }

    private void HandleHeavyAttack()
    {
        if (knifeManifest == null) return;

        isHeavy = true;
        weaponAnimator.SetTrigger("Stab");
        lastAttackTime = Time.time;

        // Duration to match animation
        Invoke("PerformOverlapAttack", 0.8f); // Timing for when the attack hits
        Invoke("EndAttack", 1.3f);
    }

    private void PerformOverlapAttack()
    {
        if (!isAttacking) return;

        lastAttackRadius = knifeManifest.bladeLength; // Adjust as needed
        lastAttackPosition = transform.position + transform.forward * 1.0f; // Slightly ahead of the knife
        Collider[] hitColliders = Physics.OverlapSphere(lastAttackPosition, lastAttackRadius);

        // Draw attack point in the Scene view
        Debug.DrawLine(transform.position, lastAttackPosition, Color.red, 1.0f);
        Debug.DrawRay(lastAttackPosition, Vector3.up * 0.5f, Color.green, 1.0f);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Bullet"))
                continue; // Skip Bullet layer

            if (hitCollider.gameObject.layer == holder.layer)
                continue; // Skip objects on the holder's layer

            var damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                int damage = isHeavy ? knifeManifest.heavyAttackDamage : knifeManifest.lightAttackDamage;
                damageable.TakeDamage(damage, false);
                Debug.Log($"Knife hit {hitCollider.name} for {damage} damage.");
            }
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
    }
}
