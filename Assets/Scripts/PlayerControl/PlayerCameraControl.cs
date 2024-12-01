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
    private Vector3 currentRecoil;                  // Current recoil offset
    private float currentVerticalRecoil = 0f;       // Current accumulated vertical recoil
    private Vector3 targetRecoil;                   // Target recoil offset

    private int currentStep;                        // Current recoil pattern index
    private bool isRecovering = false;
    private float recoilResetTimer = 0f;

    private Vector3 originalCameraPosition;

    private PlayerController playerController;

    private void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalCameraPosition = playerCamera.transform.localPosition;
        playerController = GetComponent<PlayerController>();

        if (playerController == null) { Debug.LogError("Assign player controller!"); }
    }

    private void Update()
    {
        HandleMouseLook();
        HandleCrouchCameraEffect();

        if (!playerController.currentWeapon.isReloading)
        {
            ApplyRecoil();
        }
        HandleRecoilRecover();
    }

    private void HandleMouseLook()
    {
        // Get mouse input for camera rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the camera up and down
        xRotation -= mouseY; // Adjust pitch
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp the rotation to avoid over-rotation

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the player body left and right
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleCrouchCameraEffect()
    {
        // Check if the player is crouching using the PlayerController
        bool isCrouching = playerController != null && playerController.IsCrouching();

        // Set the target camera position based on crouching
        Vector3 targetPosition = isCrouching ? originalCameraPosition + new Vector3(0f, crouchCameraOffset, 0f) : originalCameraPosition;

        // Smoothly move the camera to the target position
        playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetPosition, Time.deltaTime * crouchTransitionSpeed);
    }

    private void ApplyRecoil()
    {
        if (Input.GetMouseButton(0) && playerController.currentWeapon.IsFiring())
        {
            Weapon tempManifest = playerController.currentWeapon.weaponManifest;
            float recoilX;
            float recoilY;

            if (tempManifest.recoilPattern.Length > 1)
            {
                // comment this to avoid recoil pattern recovery
                // currentStep = tempManifest.maxAmmo + 1 - playerController.currentWeapon.currentAmmo;
                
                Debug.Log(currentStep);
                currentStep = Mathf.Clamp(currentStep, 0, tempManifest.recoilPattern.Length - 1);

                recoilX = tempManifest.recoilPattern[currentStep].x;
                recoilY = tempManifest.recoilPattern[currentStep].y;

                currentStep++;
                // Reset recovery state since we're firing
                recoilResetTimer = 0f;
                isRecovering = false;
            }
            else
            {
                float verticalRecoil = tempManifest.verticalRecoil;
                float horizontalRecoil = tempManifest.horizontalRecoil;
                
                float growthRate = tempManifest.recoilGrowthRate;
                // Incrementally increase vertical recoil within the maximum bounds
                currentVerticalRecoil = Mathf.Min(currentVerticalRecoil + growthRate * Time.deltaTime, verticalRecoil);

                recoilX = Random.Range(-horizontalRecoil, horizontalRecoil); // Horizontal
                recoilY = Random.Range(0f, verticalRecoil);          // Vertical

            }
            // Add to target recoil
            targetRecoil += new Vector3(recoilY, recoilX, 0);
        }
        else
        {// Start recoil recovery process if not firing
            if (!isRecovering)
            {
                recoilResetTimer += Time.deltaTime;
                if (recoilResetTimer >= playerController.currentWeapon.weaponManifest.recoilResetDelay)
                {
                    currentStep = 0; // Reset the recoil step
                    isRecovering = true;
                }
            }
        }
    }

    private void HandleRecoilRecover()
    {
        // Smoothly apply recoil
        currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, Time.deltaTime * 10f);

        // Apply to camera rotation
        playerCamera.transform.localEulerAngles -= new Vector3(currentRecoil.x, currentRecoil.y, 0);

        float recoilResetSpeed = playerController.currentWeapon.weaponManifest.recoilControlRate;
        // Gradually reset the recoil
        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, Time.deltaTime * recoilResetSpeed);

        // Reset the accumulated recoil when no longer firing
        if (!Input.GetMouseButton(0))
        {
            currentVerticalRecoil = 0;
        }
    }
}
