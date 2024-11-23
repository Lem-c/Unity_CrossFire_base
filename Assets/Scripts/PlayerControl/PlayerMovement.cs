using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static PlayerMovement;

public class PlayerMovement : MonoBehaviour
{
    // Singleton pattern for PlayerMovement instance
    private static PlayerMovement _instance;
    public static PlayerMovement Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerMovement>();
            }
            return _instance;
        }
    }

    private CharacterController characterController;
    private Vector3 moveDirection;

    [Header("Movement Settings")]
    private float walkSpeed = 220f;
    private float crouchSpeed = 110f;
    private float silentSpeed = 130f;
    private float jumpForce = 260f; // Separate jump force parameter
    private float gravity = 620f; // Increased gravity for faster falling
    public float crouchHeight = 1.5f;
    public float normalHeight = 2.5f;
    public float crouchTransitionSpeed = 10f; // Speed of height transition when crouching

    [Header("Footstep Settings")]
    public AudioSource footstepAudioSource;
    public float footstepInterval = 0.5f; // Time between footsteps
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

    // player state params
    private float horizontalInput;
    private float verticalInput;
    private bool isJumping;
    private bool isCrouching;
    private bool isSilentMoving;

    private bool isGrounded;

    /// <summary>
    /// Get player global state method: isCrouching
    /// </summary>
    /// <returns></returns>
    public static bool GetIsCrouching()
    {
        
        return Instance.isCrouching;
    }


    private void Awake()
    {
        // Initialize Singleton instance
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        GetInput();
        HandleMovement();
        HandleFootsteps();
        HandleJump();
        HandleCrouchHeight();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // A, D
        verticalInput = Input.GetAxis("Vertical");   // W, S
        isJumping = Input.GetButton("Jump"); // Space
        isCrouching = Input.GetKey(KeyCode.LeftControl); // LCtrl
        isSilentMoving = Input.GetKey(KeyCode.LeftShift); // LShift
    }

    private void HandleMovement()
    {
        // Determine movement speed
        float speed = walkSpeed;
        if (isCrouching && isGrounded)
        {
            speed = crouchSpeed;
        }
        else if (isSilentMoving && isGrounded)
        {
            speed = silentSpeed;
        }

        // Ensure diagonal movement speed is normalized
        Vector3 desiredMove = transform.right * horizontalInput + transform.forward * verticalInput;
        if (desiredMove.magnitude > 1)
        {
            desiredMove.Normalize();
        }

        moveDirection.x = desiredMove.x * speed;
        moveDirection.z = desiredMove.z * speed;

        // Apply gravity
        if (isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = 0f;
        }
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the player
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleJump()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && isJumping)
        {
            moveDirection.y = jumpForce; // Use jump force to control upward velocity
        }
    }

    private void HandleCrouchHeight()
    {
        // Set the target height based on crouching
        float targetHeight = isCrouching ? crouchHeight : normalHeight;

        // Smoothly adjust the character controller height
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
    }

    private void HandleFootsteps()
    {
        // Only play footstep sounds when the player is moving and grounded
        if (isGrounded && characterController.velocity.magnitude > 0.1f)
        {
            footstepTimer += Time.deltaTime;

            // Adjust the footstep interval based on movement speed
            float interval = footstepInterval;
            if (isCrouching)
            {
                interval *= 1.5f; // Slower footsteps when crouching
            }
            else if (isSilentMoving)
            {
                interval *= 1.2f; // Slightly slower when moving silently
            }

            if (footstepTimer >= interval)
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

                // Debug.Log("Ground Material Detected: " + materialName);

                // Find the matching footstep sound
                AudioClip clipToPlay = null;
                foreach (FootstepSound footstepSound in footstepSounds)
                {
                    if (footstepSound.materialName == materialName)
                    {
                        // Choose a random clip from the list
                        int randomIndex = Random.Range(0, footstepSound.footstepClips.Count);
                        if (footstepSound.footstepClips.Capacity > 1)
                        {
                            clipToPlay = footstepSound.footstepClips[randomIndex];
                        }
                        else
                        {
                            clipToPlay = defaultFootStepClips[randomIndex];
                        }
                        break;
                    }
                }

                // If no specific sound is found, use a default sound
                if (clipToPlay == null && footstepSounds.Count > 0)
                {
                    int randomIndex = Random.Range(0, footstepSounds[0].footstepClips.Count);

                    // First sound in the list as default
                    if (footstepSounds[0].footstepClips.Capacity > 1)
                    {
                        clipToPlay = footstepSounds[0].footstepClips[randomIndex];
                    }
                    else
                    {
                        clipToPlay = defaultFootStepClips[randomIndex];
                    }
                }

                // Play the footstep sound
                if (clipToPlay != null)
                {
                    // Debug.Log("Playing Footstep Sound for Material: " + materialName);
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
