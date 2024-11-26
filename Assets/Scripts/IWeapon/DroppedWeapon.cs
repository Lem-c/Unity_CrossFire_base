using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedWeapon : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 4.0f; // Range within which player can pick up the weapon

    [Header("Weapon Settings")]
    public int weaponSlotIndex; // Weapon type index: 0 - Rifle, 1 - Pistol, 2 - Shotgun, 3 - Grenade
    public string weaponName; // Name of the weapon prefab

    private Transform playerTransform;
    private PlayerController playerController;
    private bool isInRange;

    private void Update()
    {
        // Check if the player is within pickup range
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            isInRange = distanceToPlayer <= pickupRange;
            
            // Automatically pick up if player has an empty slot for this weapon type
            if (isInRange)
            {
                HandleAutomaticPickup();
            }
        }
    }

    private void HandleAutomaticPickup()
    {
        // Check if the corresponding weapon slot is empty
        if (playerController != null && playerController.weaponSlots[weaponSlotIndex] == null)
        {
            // Get the weapon model from WeaponStorage by name
            GameObject weaponPrefab = WeaponStorage.Instance.GetWeaponModelByName(weaponName);
            if (weaponPrefab != null)
            {
                // Instantiate the weapon and assign it to the player's corresponding slot
                GameObject weaponInstance = Instantiate(weaponPrefab);

                Transform weaponCameraTransform = playerTransform.Find("Main Camera");
                if (weaponCameraTransform != null)
                {
                    weaponInstance.transform.SetParent(weaponCameraTransform);
                }
                else
                {
                    Debug.LogError("Main Camera not found. Please check the hierarchy.");
                }

                WeaponController weaponController = weaponInstance.GetComponent<WeaponController>();
                // Set the weapon's local position, rotation and scale from saved data
                weaponInstance.transform.localPosition = weaponController.weaponManifest.savedLocalPosition;
                weaponInstance.transform.localRotation = weaponController.weaponManifest.savedLocalRotation;
                weaponInstance.transform.localScale = weaponController.weaponManifest.savedLocalScale;

                playerController.weaponSlots[weaponSlotIndex] = weaponController;
                weaponController.gameObject.SetActive(false);
                Destroy(gameObject); // Destroy the dropped weapon object after pickup
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            playerController = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = null;
            playerController = null;
            isInRange = false;
        }
    }
}
