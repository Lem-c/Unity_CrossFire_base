using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepAudioPlayer : MonoBehaviour
{
    [Header("Footstep Settings")]
    public AudioSource footstepAudioSource;
    public float footstepInterval = 0.05f; // Time between footsteps
    public List<AudioClip> defaultFootStepClips = new List<AudioClip>();
    private float footstepTimer;

    public LayerMask groundLayerMask; // Layers considered as ground

    [System.Serializable]
    public class FootstepSound
    {
        public string materialName;
        public List<AudioClip> footstepClips = new List<AudioClip>();
    }
    public List<FootstepSound> footstepSounds = new List<FootstepSound>();

    // Reference to the PlayerMovement script
    private PlayerController playerController;

    private void Start()
    {
        // Get the PlayerMovement component from the same GameObject or specify the player GameObject
        playerController = GetComponent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("PlayerMovement component not found on the GameObject.");
        }
    }

    private void Update()
    {
        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        if (playerController == null)
            return;

        // Only play footstep sounds when the player is moving and grounded
        if (playerController.IsGrounded() && playerController.IsMoving())
        {
            footstepTimer += Time.deltaTime;

            // Adjust the footstep interval based on movement speed
            float interval = footstepInterval;

            if (footstepTimer >= interval && !playerController.IsCrouching() && !playerController.IsSilentMoving())
            {
                // Reset the timer
                footstepTimer = 0f;

                // Play footstep sound
                PlayFootstepSound();
            }
        }
        else
        {
            // Reset the timer when not moving
            footstepTimer = 0f;

            // Ensure that no sound is played accidentally
            footstepAudioSource.Stop();
        }
    }

    private void PlayFootstepSound()
    {
        // Perform a Raycast downwards to detect the ground object
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 500f, groundLayerMask))
        {
            // Retrieve the GameObject that was hit by the Raycast
            GameObject hitObject = hit.collider.gameObject;

            // Try to get the MeshRenderer component from the hit object
            MeshRenderer meshRenderer = hitObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                // Get the material name from the MeshRenderer's sharedMaterial
                string materialName = meshRenderer.sharedMaterial != null ? meshRenderer.sharedMaterial.name : "Default";

                // Find the matching footstep sound
                AudioClip clipToPlay = null;
                foreach (FootstepSound footstepSound in footstepSounds)
                {
                    if (footstepSound.materialName == materialName)
                    {
                        // Choose a random clip from the list
                        if (footstepSound.footstepClips.Count > 0)
                        {
                            int randomIndex = Random.Range(0, footstepSound.footstepClips.Count);
                            clipToPlay = footstepSound.footstepClips[randomIndex];
                        }
                        break;
                    }
                }

                // If no specific sound is found, use a default sound
                if (clipToPlay == null && defaultFootStepClips.Count > 0)
                {
                    int randomIndex = Random.Range(0, defaultFootStepClips.Count);
                    clipToPlay = defaultFootStepClips[randomIndex];
                }

                // Play the footstep sound
                if (clipToPlay != null)
                {
                    footstepAudioSource.PlayOneShot(clipToPlay);
                }
            }
            else
            {
                Debug.LogWarning("No MeshRenderer found on the hit object.");
            }
        }
        else
        {
            Debug.LogWarning("No ground detected beneath player.");
        }
    }
}
