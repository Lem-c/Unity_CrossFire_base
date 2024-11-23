using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField]
    public GameObject bulletPrefab; // Reference to the bullet prefab
    public AnimationCurve damageFalloff; // Curve to control how damage decreases over distance

    // Public properties to define bullet attributes
    protected float velocity;
    protected float damage;
    protected string bulletType;
    protected float maxRange;

    protected Vector3 startPosition;
    private Rigidbody rb;

    protected GameObject shooter { set; get; } // Reference to the shooter to avoid self-hit
    // public method to set the shooter
    public void SetShooter(GameObject shooterObject)
    {
        shooter = shooterObject;
    }

    // Initialize bullet properties
    protected virtual void Start()
    {
        startPosition = transform.position;
        AssignData();

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // Disable gravity effect on the bullet
        }
    }

    protected void InitiBullet(float v, float damage, string type, float maxRange)
    {
        velocity = v;
        this.damage = damage;
        bulletType = type;
        this.maxRange = maxRange;
    }

    // Update bullet movement each frame
    protected virtual void Update()
    {
        // Move bullet forward at the defined velocity
        transform.Translate(Vector3.forward * velocity * Time.deltaTime);

        // Calculate distance travelled
        float distanceTravelled = Vector3.Distance(startPosition, transform.position);

        // Destroy bullet if it exceeds max range
        if (distanceTravelled >= maxRange)
        {
            Destroy(gameObject);
        }
    }

    // Method to calculate damage based on distance
    public float CalculateDamage(float distance)
    {
        if (damageFalloff != null)
        {
            return damage * damageFalloff.Evaluate(distance / maxRange);
        }
        return damage;
    }

    // Abstract method to handle collision
    protected abstract void OnCollisionEnter(Collision collision);
    protected abstract void AssignData();
}

// Interface to represent damageable objects
public interface IDamageable
{
    void TakeDamage(float damage, bool isLog);
}
