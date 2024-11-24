// A specific bullet type: pistol
using UnityEngine;

public class PistolBullet : Bullet
{
    private Vector3 previousPosition;

    protected override void AssignData()
    {
        InitiBullet(500f, 45f, "pistol", 10000f);
    }
    protected override void Start()
    {
        base.Start();
        previousPosition = transform.position;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Perform raycasting for high-speed collision detection
        Vector3 currentPosition = transform.position;
        Vector3 direction = currentPosition - previousPosition;
        float distance = direction.magnitude;

        if (distance > 0)
        {
            Ray ray = new Ray(previousPosition, direction.normalized);
            if (Physics.Raycast(ray, out RaycastHit hit, distance))
            {
                HandleCollision(hit.collider);
            }
        }

        previousPosition = transform.position;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
    }

    private void HandleCollision(Collider collider)
    {
        // Ignore objects on the Bullet layer
        if (collider.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            return;
        }

        // Return immediately if the other object's layer matches the shooter's layer
        if (collider.gameObject.layer == shooter.layer)
        {
            return;
        }

        Debug.Log($"Collision detected with {collider.gameObject.name}");

        // Check if the target implements IDamageable
        IDamageable target = collider.GetComponent<IDamageable>();
        if (target != null)
        {
            float distanceTravelled = Vector3.Distance(startPosition, transform.position);
            float calculatedDamage = CalculateDamage(distanceTravelled);
            target.TakeDamage(calculatedDamage, true);
            Debug.Log($"Damage dealt to {collider.gameObject.name} of {calculatedDamage}");
        }

        // Destroy the bullet after collision
        Destroy(gameObject);
    }

}