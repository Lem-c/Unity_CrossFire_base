using UnityEngine;

public class PlayerState : MonoBehaviour, IDamageable
{
    [SerializeField]
    public float maxHealth;
    public float maxArmor;
    public float healthView = 0;
    public float armorView = 0;
    public Animator animator;

    [Header("Body Parts Colliders")]
    public Collider headCollider;
    public Collider bodyCollider;
    public Collider leftLegCollider;
    public Collider rightLegCollider;
    public Collider leftArmCollider;
    public Collider rightArmCollider;


    protected float currentHealth;
    protected float currentArmor;

    // How much percent of damage left
    private float armorLevel = 0.8f;

    public float getCurrentHealth()
    {
        return currentHealth;
    }

    public float getCurrentArmor()
    {
        return currentArmor;
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;

        // get animator
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        healthView = getCurrentHealth();
        armorView = getCurrentArmor();
    }

    public void TakeDamage(float damage)
    {
        CalculateHealthLeft(damage);

        if (currentHealth <= 0)
        {
            DisableAllColliders();
            currentHealth = 0;

            if (animator != null)
            {
                // Handle player death
                animator.SetBool("isDie", true);
            }

            // destroy after few seconds
            Destroy(gameObject);
        }
    }

    // Method to take damage
    public virtual void TakeDamage(float damage, bool isLog=true)
    {
        CalculateHealthLeft(damage);

        if (isLog)
        {
            Debug.Log($"Got hit! Health remaining: {currentHealth}");
        }

        if (currentHealth <= 0)
        {
            DisableAllColliders();
            currentHealth = 0;

            if (animator != null)
            {
                // Handle player death
                animator.SetBool("isDie", true);
                if (isLog) Debug.Log("Die!!");
            }

            // destroy after few seconds
            Destroy(gameObject, 1f);
        }
    }

    protected void DisableAllColliders()
    {
        if (headCollider) headCollider.enabled = false;
        if (bodyCollider) bodyCollider.enabled = false;
        if (leftLegCollider) leftLegCollider.enabled = false;
        if (rightLegCollider) rightLegCollider.enabled = false;
        if (leftArmCollider) leftArmCollider.enabled = false;
        if (rightArmCollider) rightArmCollider.enabled = false;
    }

    protected void CalculateHealthLeft(float damage)
    {
        if (currentArmor > 0)
        {
            float remainingDamage = armorLevel * damage;
            currentArmor -= damage;

            if (currentArmor < 0)
            {
                currentArmor = 0;
            }

            if (remainingDamage > 0)
            {
                currentHealth -= remainingDamage;
            }
        }
        else
        {
            currentHealth -= damage;
        }
    }
}
