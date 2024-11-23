using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerWeaponUI : MonoBehaviour
{
    public TextMeshProUGUI ammoCountText;

    void Update()
    {
        ammoCountText.text = WeaponController.displayAmmo + " / ?";
    }
}
