// A specific bullet type: pistol
using UnityEngine;

public class PistolBullet : Bullet
{
    protected override void AssignData()
    {
        InitiBullet(500f, 45f, "pistol", 10000f);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        // Return immediately if the other object's layer matches the shooter's layer
        if (other.gameObject.layer == shooter.layer)
        {
            return;
        }

        // Debug.Log($"Collision detected with {other.gameObject.name}");


        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            float distanceTravelled = Vector3.Distance(startPosition, transform.position);
            float calculatedDamage = CalculateDamage(distanceTravelled);
            target.TakeDamage(calculatedDamage, true);
            Debug.Log($"Damage dealt to {other.gameObject.name} of {calculatedDamage}");
        }

        Destroy(gameObject);
    }

}