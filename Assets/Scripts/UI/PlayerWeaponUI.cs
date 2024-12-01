using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerWeaponUI : MonoBehaviour
{
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI ammoCountText;

    // weapon icon
    public Image weaponImage;

    void Update()
    {
        weaponNameText.text = WeaponController.displayWeapon;
        ammoCountText.text = WeaponController.displayAmmo;

        // Update weapon image
        if (WeaponController.currentWeaponSprite != null)
        {
            weaponImage.sprite = WeaponController.currentWeaponSprite; // Set weapon image sprite
            weaponImage.enabled = true; // Ensure the image is visible
        }
        else
        {
            weaponImage.enabled = false; // Hide image if no sprite is available
        }
    }
}
