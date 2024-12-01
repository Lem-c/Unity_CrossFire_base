using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
{
    private List<WeaponController[]> bags = new List<WeaponController[]>();
    private WeaponController[] currentBag;

    public Transform weaponParent;
    private GameObject topParent;

    public List<GameObject> riflePrefabs = new List<GameObject>();
    public List<GameObject> pistolPrefabs = new List<GameObject>();
    public List<GameObject> knifePrefabs = new List<GameObject>();
    public List<GameObject> grenadePrefabs = new List<GameObject>();

    public bool canSwitchBag = true;

    private void Awake()
    {
        GetTopParents();
        InitializeBags();
        // Default select the first bag
        if (bags.Count > 0)
        {
            currentBag = bags[0];
        }
    }

    private void InitializeBags()
    {
        // Initialize bags with 6 default weapon sets
        for (int i = 0; i < 6; i++)
        {
            WeaponController[] weaponSet = new WeaponController[4];

            // Add rifle
            if (riflePrefabs.Count > i)
            {
                GameObject rifleInstance = Instantiate(riflePrefabs[i], weaponParent);
                rifleInstance.SetActive(false);  // Ensure it's not active immediately

                weaponSet[0] = rifleInstance.GetComponent<WeaponController>();
                weaponSet[0].holder = topParent;

                Weapon tempManifest = weaponSet[0].weaponManifest;
                // Set transform
                rifleInstance.transform.localPosition = tempManifest.savedLocalPosition;
                rifleInstance.transform.localRotation = tempManifest.savedLocalRotation;
                rifleInstance.transform.localScale = tempManifest.savedLocalScale;

            }

            // Add pistol
            if (pistolPrefabs.Count > i)
            {
                GameObject pistolInstance = Instantiate(pistolPrefabs[i], weaponParent);
                pistolInstance.SetActive(false);  // Ensure it's not active immediately
                weaponSet[1] = pistolInstance.GetComponent<WeaponController>();
                weaponSet[1].holder = topParent;

                Weapon tempManifest = weaponSet[1].weaponManifest;
                // Set transform
                pistolInstance.transform.localPosition = tempManifest.savedLocalPosition;
                pistolInstance.transform.localRotation = tempManifest.savedLocalRotation;
                pistolInstance.transform.localScale = tempManifest.savedLocalScale;
            }

            // Add knife
            if (knifePrefabs.Count > i)
            {
                GameObject knifeInstance = Instantiate(knifePrefabs[i], weaponParent);
                knifeInstance.SetActive(false);  // Ensure it's not active immediately
                weaponSet[2] = knifeInstance.GetComponent<KnifeWeaponController>();
                weaponSet[2].holder = topParent;

                Weapon tempManifest = weaponSet[2].weaponManifest;
                // Set transform
                knifeInstance.transform.localPosition = tempManifest.savedLocalPosition;
                knifeInstance.transform.localRotation = tempManifest.savedLocalRotation;
                knifeInstance.transform.localScale = tempManifest.savedLocalScale;
            }

            // Add grenade
            if (grenadePrefabs.Count > i)
            {
                GameObject grenadeInstance = Instantiate(grenadePrefabs[i], weaponParent);
                grenadeInstance.SetActive(false);  // Ensure it's not active immediately
                weaponSet[3] = grenadeInstance.GetComponent<WeaponController>();
                weaponSet[3].holder = topParent;

                Weapon tempManifest = weaponSet[2].weaponManifest;
                // Set transform
                grenadeInstance.transform.localPosition = tempManifest.savedLocalPosition;
                grenadeInstance.transform.localRotation = tempManifest.savedLocalRotation;
                grenadeInstance.transform.localScale = tempManifest.savedLocalScale;
            }

            bags.Add(weaponSet);
        }
    }

    private void GetTopParents()
    {
        // Traverse up the hierarchy to find the top-level parent
        Transform currentTransform = weaponParent;

        while (currentTransform.parent != null)
        {
            currentTransform = currentTransform.parent;
        }

        topParent = currentTransform.gameObject;

        if (topParent == null)
        {
            Debug.LogWarning("Top-level parent GameObject not found!");
        }
    }

    private void InstantiateWeapons(WeaponController[] weaponSet)
    {
        // Clear previous weapons if any
        foreach (Transform child in weaponParent)
        {
            Destroy(child.gameObject);
        }

        // Instantiate weapons from the selected bag
        foreach (WeaponController weapon in weaponSet)
        {
            if (weapon != null)
            {
                Debug.Log(weapon);
                GameObject newWeapon = Instantiate(weapon.gameObject, weaponParent);
                newWeapon.SetActive(false);
                WeaponController weaponInstance = newWeapon.GetComponent<WeaponController>();
                // set holder and manifest
                weaponInstance.holder = topParent;
                weaponInstance.weaponManifest = weapon.weaponManifest;
            }
        }
    }

    public WeaponController[] GetCurrentBagWeapons()
    {
        return currentBag;
    }

    public void SetCanSwitchBag(bool value)
    {
        canSwitchBag = value;
    }

    public void SelectBag(int bagIndex)
    {
        if (bagIndex >= 0 && bagIndex < bags.Count)
        {
            currentBag = bags[bagIndex];
            // InstantiateWeapons(currentBag);
            Debug.Log("Bag " + (bagIndex + 1) + " selected");
        }
    }
}
