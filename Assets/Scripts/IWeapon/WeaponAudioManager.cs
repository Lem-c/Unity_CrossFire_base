using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAudioManager : MonoBehaviour
{
    // Audio clips for weapon actions
    [SerializeField] private AudioClip drawClip;
    [SerializeField] private AudioClip fireClip;
    [SerializeField] private AudioClip reloadClip_1;
    [SerializeField] private AudioClip reloadClip_2;
    [SerializeField] private AudioClip reloadClip_3;

    // Private audio source for this weapon
    private AudioSource audioSource;

    private void Awake()
    {
        // Ensure an AudioSource component is present
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Optional: Customize AudioSource settings (e.g., volume, pitch)
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0.0f; // 2D sound
    }

    // Public methods to play audio clips
    public void PlayDrawSound()
    {
        PlaySound(drawClip);
    }

    public void PlayFireSound()
    {
        PlaySound(fireClip);
    }

    public void PlayStartReloadSound()
    {
        PlaySound(reloadClip_1);
    }

    public void PlayReloadMagSound() { 
        PlaySound(reloadClip_2);
    }

    public void PlayStopReloadSound()
    {
        PlaySound(reloadClip_3);
    }

    // Private helper method to play a sound
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Audio clip is missing or AudioSource is not set on {gameObject.name}");
        }
    }

    public void StartFullAuto(bool isFiring, float fireRate)
    {
        if (!isFiring)
        {
            isFiring = true;
            StartCoroutine(PlayFullAutoSound(isFiring, fireRate));
        }
    }

    public void StopFullAuto(bool isFiring, float fireRate)
    {
        isFiring = false;
        StopCoroutine(PlayFullAutoSound(isFiring, fireRate));
    }

    private IEnumerator PlayFullAutoSound(bool isFiring, float fireRate)
    {
        while (isFiring)
        {
            if (fireClip != null)
            {
                audioSource.PlayOneShot(fireClip);
            }
            yield return new WaitForSeconds(fireRate); // Wait for the fire rate interval
        }
    }
}
