using UnityEngine;

public interface IWeaponState
{
    void HandleState(WeaponController weaponController);
}

public class WeaponIdleState : IWeaponState
{
    public void HandleState(WeaponController weaponController)
    {
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            weaponController.SetAnimatorInteger("Walk", 1); // Walk
        }
        else
        {
            weaponController.SetAnimatorInteger("Walk", 2); // Idle
        }
    }
}

public class WeaponShootState : IWeaponState
{
    public void HandleState(WeaponController weaponController)
    {
        if (weaponController.currentAmmo > 0)
        {
            weaponController.Shoot();
        }
        else
        {
            weaponController.SetState(new WeaponReloadState());
        }
    }
}

public class WeaponReloadState : IWeaponState
{
    public void HandleState(WeaponController weaponController)
    {
        weaponController.Reload();
    }
}

