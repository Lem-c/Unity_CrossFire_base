using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour, IDamageable
{
    [SerializeField]
    public float maxHealth = 100f;
    public float maxArmor = 0;
    public float healthView = 0;

    protected float currentHealth;
    protected float currentArmor;

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
    }

    private void Update()
    {
        healthView = getCurrentHealth();
    }

    // Method to take damage
    public virtual void TakeDamage(float damage, bool isLog=true)
    {
        if (currentArmor > 0)
        {
            float remainingDamage = damage - currentArmor;
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
            // Handle player death
            Destroy(gameObject);
        }
    }
}

