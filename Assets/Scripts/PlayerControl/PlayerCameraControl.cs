using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraControl : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera playerCamera;
    public float mouseSensitivity = 100f;
    public float crouchCameraOffset = -0.5f; // Offset for the camera when crouching
    public float crouchTransitionSpeed = 10f; // Speed of the camera transition when crouching

    private float xRotation = 0f;

    private Vector3 originalCameraPosition;

    private PlayerController playerController;

    private void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalCameraPosition = playerCamera.transform.localPosition;
        playerController = GetComponent<PlayerController>();

        if (playerController == null) { Debug.LogError("Assgin player controller!"); }
    }

    private void Update()
    {
        HandleMouseLook();
        HandleCrouchCameraEffect();
    }

    private void HandleMouseLook()
    {
        // Get mouse input for camera rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the camera up and down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp the rotation to avoid over-rotation

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the player body left and right
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleCrouchCameraEffect()
    {
        // Check if the player is crouching using the PlayerMovement static method
        bool isCrouching = playerController != null && playerController.IsCrouching();

        // Set the target camera position based on crouching
        Vector3 targetPosition = isCrouching ? originalCameraPosition + new Vector3(0f, crouchCameraOffset, 0f) : originalCameraPosition;

        // Smoothly move the camera to the target position
        playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetPosition, Time.deltaTime * crouchTransitionSpeed);
    }
}