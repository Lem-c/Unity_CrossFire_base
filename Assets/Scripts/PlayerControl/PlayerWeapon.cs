using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponController mainWeapon;
    public WeaponController pistol;
    public WeaponController knife;
    public WeaponController grenade;

    private WeaponController currentWeapon;

    void Start()
    {
        // Start with the main weapon
        SwitchWeapon(pistol);
    }

    void Update()
    {
        HandleWeaponSwitching();
        if (currentWeapon != null)
        {
            currentWeapon.HandleWeaponInput();
        }
    }

    private void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(mainWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(pistol);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(knife);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchWeapon(grenade);
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
}
