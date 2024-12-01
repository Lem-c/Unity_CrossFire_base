using UnityEngine;

public class HitBox : MonoBehaviour
{
    public float damageMultiplier = 1.0f;
    public PlayerState playerState;

    void Start()
    {
        if (playerState == null)
        {
            Debug.LogError("PlayerState not found on parent object!");
        }
    }

    public void ApplyDamage(float baseDamage, bool isLog = false)
    {
        if (playerState != null)
        {
            float finalDamage = baseDamage * damageMultiplier;
            playerState.TakeDamage(finalDamage, isLog);
        }
    }
}
