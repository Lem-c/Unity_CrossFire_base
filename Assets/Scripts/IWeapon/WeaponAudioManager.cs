using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAudioManager : MonoBehaviour
{
    // Singleton instance for global safe access
    private static WeaponAudioManager _instance;
    public static WeaponAudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<WeaponAudioManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("WeaponAudioManager");
                    _instance = go.AddComponent<WeaponAudioManager>();
                }
            }
            return _instance;
        }
    }

    // AudioClips for different weapon actions
    [SerializeField]
    public AudioClip drawClip;
    public AudioClip fireClip;
    public AudioClip reloadClip;

    private AudioSource audioSource;

    private void Awake()
    {
        // Ensure only one instance of WeaponAudioManager exists
        if (_instance == null)
        {
            _instance = this;
            if (transform.parent == null)
            {
                // only apply to root GameObjects
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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

    public void PlayReloadSound()
    {
        PlaySound(reloadClip);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.Log("Audio source missed.");
        }
    }
}

// WeaponAudioManager.Instance.PlayFireSound();
// WeaponAudioManager.Instance.PlayReloadSound();
