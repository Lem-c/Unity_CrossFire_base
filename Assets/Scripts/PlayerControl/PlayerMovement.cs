using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float walkSpeed = 25f;
    private float crouchSpeed = 6.5f;
    private float silentSpeed = 10f;
    private float jumpForce = 30f; // Separate jump force parameter
    private float gravity = 100f; // Increased gravity for faster falling
    public float crouchHeight = 1.5f;
    public float normalHeight = 2.5f;
    public float crouchTransitionSpeed = 10f; // Speed of height transition when crouching

    // Player state parameters
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
        HandleJump();
        HandleCrouchHeight();
        // Footstep handling is now managed by FootStepAudioPlayer
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // A, D
        verticalInput = Input.GetAxis("Vertical");     // W, S
        isJumping = Input.GetButton("Jump");           // Space
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

    // Provide methods for FootStepAudioPlayer to access necessary information
    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsMoving()
    {
        // Use a small threshold to determine movement
        return characterController.velocity.magnitude > 0.1f;
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }

    public bool IsSilentMoving()
    {
        return isSilentMoving;
    }
}
