using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponController mainWeapon;
    public WeaponController pistol;
    public WeaponController knife;
    public WeaponController grenade;

    private WeaponController currentWeapon = null;

    void Start()
    {
        OnBagInitialized();
        // Start with the main weapon
        SwitchWeapon(mainWeapon);
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

    private void OnBagInitialized()
    {
        if (mainWeapon != null)
        {
            mainWeapon.gameObject.SetActive(false);
        }

        if(pistol != null)
        {
            pistol.gameObject.SetActive(false);
        }

        if(knife != null)
        {
            knife.gameObject.SetActive(false);
        }

        if(grenade != null)
        {
            grenade.gameObject.SetActive(false);
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
