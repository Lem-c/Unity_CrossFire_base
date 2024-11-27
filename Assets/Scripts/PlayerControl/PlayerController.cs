using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Vector3 moveDirection;

    [Header("Movement Settings")]
    private float walkSpeed = 25f;
    private float crouchSpeed = 6.5f;
    private float silentSpeed = 10f;
    private float jumpForce = 30f;
    private float gravity = 100f;
    public float crouchHeight = 1.5f;
    public float normalHeight = 2.5f;
    public float crouchTransitionSpeed = 10f;
    public float stepOverHeight = 0.5f;

    // Player state parameters
    private float currentHeight;
    private float horizontalInput;
    private float verticalInput;
    private bool isJumping;
    private bool isCrouching;
    private bool isSilentMoving;
    private bool isGrounded;

    [Header("Weapon Settings")]
    public WeaponController[] weaponSlots = new WeaponController[4];
    public Transform modelCenter;
    public WeaponController currentWeapon = null;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        currentHeight = normalHeight;

        OnBagInitialized();

        if (weaponSlots[0] != null)
        {
            SwitchWeapon(weaponSlots[0]);
        }
    }

    private void Update()
    {
        GetInput();
        HandleMovement();
        HandleJump();
        HandleCrouchHeight();
        HandleWeaponSwitching();
        HandleWeaponDrop();
        /*if (currentWeapon != null)
        {
            currentWeapon.HandleWeaponInput();
        }*/
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isJumping = Input.GetButton("Jump");
        isCrouching = Input.GetKey(KeyCode.LeftControl);
        isSilentMoving = Input.GetKey(KeyCode.LeftShift);
    }

    private void HandleMovement()
    {
        float speed = walkSpeed;
        if (isCrouching && isGrounded)
        {
            speed = crouchSpeed;
        }
        else if (isSilentMoving && isGrounded)
        {
            speed = silentSpeed;
        }

        Vector3 desiredMove = transform.right * horizontalInput + transform.forward * verticalInput;
        if (desiredMove.magnitude > 1)
        {
            desiredMove.Normalize();
        }

        moveDirection.x = desiredMove.x * speed;
        moveDirection.z = desiredMove.z * speed;

        if (isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = 0f;
        }
        moveDirection.y -= gravity * Time.deltaTime;

        HandleStepOver();

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleJump()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && isJumping)
        {
            moveDirection.y = jumpForce;
        }
    }

    private void HandleStepOver()
    {
        // Check for obstacles to step over
        Vector3 rayOriginLow = new Vector3(transform.position.x, transform.position.y - currentHeight - 1f, transform.position.z);
        Vector3 rayOriginHigh = new Vector3(transform.position.x, transform.position.y - currentHeight - 1f + stepOverHeight / 2, transform.position.z);
        float rayLength = 2f; // Length of ray to check obstacles in front of player

        RaycastHit hitLow;
        RaycastHit hitHigh;

        if (Physics.Raycast(rayOriginLow, transform.forward, out hitLow, rayLength))
        {
            // Check if obstacle height is low enough to step over
            if (hitLow.collider != null)
            {
                // Check if there's no obstacle blocking the higher ray
                if (!Physics.Raycast(rayOriginHigh, transform.forward, out hitHigh, rayLength))
                {
                    if(!isGrounded) { return; }
                    if(verticalInput == 0 && horizontalInput == 0) { return; }

                    moveDirection.y = jumpForce * 0.5f; // A small hop to cross the step
                }
            }
        }
    }

    private void HandleCrouchHeight()
    {
        currentHeight = isCrouching ? crouchHeight : normalHeight;
        characterController.height = Mathf.Lerp(characterController.height, currentHeight, Time.deltaTime * crouchTransitionSpeed);
    }

    private void HandleWeaponSwitching()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && weaponSlots[i] != null)
            {
                SwitchWeapon(weaponSlots[i]);
                break;
            }
        }
    }

    private void HandleWeaponDrop()
    {
        if (Input.GetKeyDown(KeyCode.G) && currentWeapon != null && currentWeapon != weaponSlots[2] && currentWeapon != weaponSlots[3])
        {
            DropWeapon(currentWeapon);
            SwitchToNextAvailableWeapon();
        }
    }

    private void DropWeapon(WeaponController weaponToDrop)
    {
        CreateDroppedWeaponPrefab(weaponToDrop);
        weaponToDrop.gameObject.SetActive(false);
        weaponToDrop.transform.position = transform.position + transform.forward;
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] == weaponToDrop)
            {
                weaponSlots[i] = null;
                break;
            }
        }
    }

    private void SwitchToNextAvailableWeapon()
    {
        foreach (WeaponController weapon in weaponSlots)
        {
            if (weapon != null && weapon != currentWeapon)
            {
                SwitchWeapon(weapon);
                return;
            }
        }
        currentWeapon = null;
    }

    private void OnBagInitialized()
    {
        foreach (WeaponController weapon in weaponSlots)
        {
            if (weapon != null)
            {
                weapon.gameObject.SetActive(false);
            }
        }
    }

    private void SwitchWeapon(WeaponController newWeapon)
    {
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }

        currentWeapon = newWeapon;

        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(true);
            currentWeapon.InitializeWeapon();
        }
    }

    private void CreateDroppedWeaponPrefab(WeaponController weaponToDrop)
    {
        if (weaponToDrop != null && modelCenter != null)
        {
            string weaponName = weaponToDrop.weaponManifest.weaponName;
            string groundWeaponName = "Ground_" + weaponName;

            GameObject weaponPrefab = WeaponStorage.Instance.GetDroppedWeaponModelByName(groundWeaponName);
            if (weaponPrefab != null)
            {
                Vector3 dropPosition = modelCenter.position + modelCenter.forward * 1.0f + Vector3.up * 5f;
                Quaternion dropRotation = Quaternion.Euler(0, 90, 90);

                GameObject droppedWeapon = Instantiate(weaponPrefab, dropPosition, dropRotation);
                droppedWeapon.name = groundWeaponName;

                Rigidbody rb = droppedWeapon.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = droppedWeapon.AddComponent<Rigidbody>();
                }
                rb.mass = 10f;
                rb.useGravity = true;

                Vector3 throwDirection = modelCenter.forward + Vector3.up * 0.5f;
                float throwForce = 100f;
                rb.AddForce(throwDirection.normalized * throwForce, ForceMode.Impulse);
            }
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsMoving()
    {
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
