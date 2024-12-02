using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSwitch : MonoBehaviour
{
    [Header("Weapon Switch Components")]
    public Animator animator;
    // Get player current weapons in bag
    public PlayerController playerController;

    // weapon icons
    public Image mainWeaponImage;
    public Image pistolWeaponImage;
    public Image knifeWeaponImage;
    public Image throwWeaponImage;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null) { Debug.LogError("animator not assigned"); }
        if (playerController == null) { Debug.LogError("pc not assigned"); }

        GetAllWeaponImages();
    }

    void Update()
    {
        GetAllWeaponImages();

        CheckEmptyImage(mainWeaponImage);
        CheckEmptyImage(knifeWeaponImage);
        CheckEmptyImage(pistolWeaponImage);
        CheckEmptyImage(throwWeaponImage);
    }

    private void CheckEmptyImage(Image img)
    {
        if (img.sprite == null) 
        { 
            img.enabled = false;
            return;
        }

        img.enabled =  true;
    }

    public void GetAllWeaponImages()
    {
        foreach (WeaponController wc in playerController.weaponSlots)
        {
            if (wc == null) continue;

            switch(wc.weaponManifest.weaponType)
            {
                case WeaponType.Rifle:
                    mainWeaponImage.sprite = wc.weaponManifest.weaponImage; break;

                case WeaponType.Pistol:
                    pistolWeaponImage.sprite = wc.weaponManifest.weaponImage;break;

                case WeaponType.Knife:
                    knifeWeaponImage.sprite =wc.weaponManifest.weaponImage;break;

                default:
                    Debug.LogWarning("Check weapon manifest type assignment!");
                    break;
            }

        }
    }

    public void PlaySwitch(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Rifle:
                animator.Play("Move_0");
                break;

            case WeaponType.Pistol:
                animator.Play("Move_1");
                break;
            case WeaponType.Knife:
                animator.Play("Move_2");
                break;
            case WeaponType.Throwing:
                animator.Play("Move_3");
                break;

            default: break;
        }
    }

    public void RemoveWeaponSprite(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Rifle:
                mainWeaponImage.sprite = null;
                break;

            case WeaponType.Pistol:
                pistolWeaponImage.sprite = null;
                break;

            default: break;
        }
    }

}
