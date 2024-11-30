using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField]
    public AnimationCurve damageFalloff; // Curve to control how damage decreases over distance

    // Public properties to define bullet attributes
    protected float velocity;
    [SerializeField] protected float damage;
    protected string bulletType;
    protected float maxRange;

    protected Vector3 startPosition;
    protected Vector3 previousPosition;
    private Rigidbody rb;

    [SerializeField] protected GameObject markPrefab; // Prefab for wall marks
    [SerializeField] protected int maxMarks = 30; // Maximum number of marks allowed
    private static Queue<GameObject> marksQueue = new Queue<GameObject>();

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
        previousPosition = startPosition;
        AssignData();

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // Disable gravity effect on the bullet
        }
    }

    protected void InitiBullet(float v, string type, float maxRange)
    {
        velocity = v;
        bulletType = type;
        this.maxRange = maxRange;
    }

    // Update bullet movement each frame
    protected virtual void FixedUpdate()
    {
        if (rb != null)
        {
            // Move the bullet using Rigidbody for consistent physics handling
            rb.velocity = transform.forward * velocity;
        }
        else
        {
            // Fallback to manual movement
            transform.Translate(Vector3.forward * velocity * Time.deltaTime);
        }

        // Destroy bullet if it exceeds max range
        float distanceTravelled = Vector3.Distance(startPosition, transform.position);
        if (distanceTravelled >= maxRange)
        {
            Destroy(gameObject);
        }
    }

    protected void HandleRaycastHit()
    {
        // Perform raycasting for high-speed collision detection
        Vector3 currentPosition = transform.position;
        Vector3 direction = currentPosition - previousPosition;
        float distance = direction.magnitude;

        if (distance > 0)
        {
            Ray ray = new Ray(previousPosition, direction.normalized);
            if (Physics.Raycast(ray, out RaycastHit hit, distance))
            {
                HandleCollision(hit);
            }
        }

        previousPosition = transform.position;
    }

    protected void HandleCollision(RaycastHit hit)
    {
        Collider collider = hit.collider;

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

        // Debug.Log($"Collision detected with {collider.gameObject.name}");

        // Check if the target implements IDamageable
        IDamageable target = collider.GetComponent<IDamageable>();
        if (target != null)
        {
            float distanceTravelled = Vector3.Distance(startPosition, transform.position);
            float calculatedDamage = CalculateDamage(distanceTravelled);
            target.TakeDamage(calculatedDamage, false);
        }

        if (collider.gameObject.layer == LayerMask.NameToLayer("Map"))
        {
            // Add markers
            GenerateMark(hit.point, hit.normal, hit.collider.transform);
        }

        // Destroy the bullet after collision
        Destroy(gameObject);
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

    /// <summary>
    /// Handles mark creation
    /// </summary>
    /// <param name="position"></param>
    /// <param name="normal"></param>
    /// <param name="parent"></param>
    protected void GenerateMark(Vector3 position, Vector3 normal, Transform parent)
    {
        if (markPrefab == null) return;

        GameObject mark = Instantiate(markPrefab, position, Quaternion.LookRotation(normal));
        mark.transform.SetParent(parent);

        marksQueue.Enqueue(mark);
        if (marksQueue.Count > maxMarks)
        {
            GameObject oldestMark = marksQueue.Dequeue();
            Destroy(oldestMark);
        }
    }

    // Abstract method to handle collision
    protected abstract void OnTriggerEnter(Collider other);
    protected abstract void AssignData();
}

// Interface to represent damageable objects
public interface IDamageable
{
    void TakeDamage(float damage, bool isLog);
}
