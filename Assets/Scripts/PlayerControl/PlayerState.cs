using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour, IDamageable
{
    [SerializeField]
    public float maxHealth;
    public float maxArmor;
    public float healthView = 0;
    public float armorView = 0;
    public Animator animator;

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
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        healthView = getCurrentHealth();
        armorView = getCurrentArmor();
    }

    // Method to take damage
    public virtual void TakeDamage(float damage, bool isLog=true)
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

        if (isLog)
        {
            Debug.Log($"Got hit! Health remaining: {currentHealth}");
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            if (animator != null)
            {
                // Handle player death
                animator.SetBool("isDie", true);
            }

            if( gameObject.GetComponent< Collider>())
            {
                gameObject.GetComponent<Collider>().enabled = false;
            }

            // destroy after few seconds
            Destroy(gameObject, 1.5f);
        }
    }
}

