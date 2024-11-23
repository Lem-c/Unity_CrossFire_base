// A specific bullet type: pistol
using UnityEngine;

public class PistolBullet : Bullet
{
    protected override void AssignData()
    {
        InitiBullet(5000f, 45f, "pistol", 5000f);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        // Avoid hitting the shooter itself
        if (collision.gameObject == shooter)
        {
            return;
        }

        // Debug.Log($"Bullet hit: {collision.gameObject.name} at position: {collision.contacts[0].point}");

        // Handle collision logic, such as applying damage to the target
        IDamageable target = collision.gameObject.GetComponent<IDamageable>();
        if (target != null)
        {
            float distanceTravelled = Vector3.Distance(startPosition, transform.position);
            float calculatedDamage = CalculateDamage(distanceTravelled);
            target.TakeDamage(calculatedDamage, true);
            Debug.Log($"Damage dealt to {collision.gameObject.name} of {calculatedDamage}");
        }

        // Destroy the bullet after collision
        Destroy(gameObject);
    }
}