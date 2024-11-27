using UnityEngine;

public class PlayerKillCount : MonoBehaviour
{
    public static PlayerKillCount Instance; // Singleton instance

    public int killCount = 0; // Counter for destroyed enemies
    public AudioClip killStreakSound_01; // Sound to be played after killing two enemies
    public AudioClip killStreakSound_02; // Sound to be played after killing three enemies

    private AudioSource audioSource;

    void Awake()
    {
        // Ensure only one instance of PlayerKillCount exists
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void EnemyDestroyed()
    {
        killCount++;
        Debug.Log($"Enemies destroyed: {killCount}");

        if (killCount >= 2)
        {
            PlayKillStreakSound(killCount);
        }
    }

    private void PlayKillStreakSound(int kNum)
    {
        if (killStreakSound_01 != null && killStreakSound_02 != null)
        {
            switch (kNum)
            {
                case 2:
                    audioSource.clip = killStreakSound_01;
                    audioSource.loop = false;
                    audioSource.Play();
                    break;
                case 3:
                    audioSource.clip = killStreakSound_02;
                    audioSource.loop = false;
                    audioSource.Play();
                    break;
                default: 
                    break;
            }
        }
        else
        {
            Debug.LogWarning("Kill streak sound clip is not assigned.");
        }
    }
}
