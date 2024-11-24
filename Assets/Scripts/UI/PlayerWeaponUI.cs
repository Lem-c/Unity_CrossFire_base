using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerWeaponUI : MonoBehaviour
{
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI ammoCountText;

    void Update()
    {
        weaponNameText.text = WeaponController.displayWeapon;
        ammoCountText.text = WeaponController.displayAmmo + " / ?";
    }
}
