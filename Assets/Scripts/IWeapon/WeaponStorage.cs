using System.Collections.Generic;
using UnityEngine;

public class WeaponStorage : MonoBehaviour
{
    public static WeaponStorage Instance;
    public List<GameObject> groundWeaponModels;
    public List<GameObject> WeaponModels;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetDroppedWeaponModelByName(string weaponName)
    {
        foreach (GameObject weaponModel in groundWeaponModels)
        {
            if (weaponModel.name == weaponName)
            {
                Debug.Log(weaponName);
                return weaponModel;
            }
        }
        return null;
    }

    public GameObject GetWeaponModelByName(string weaponName)
    {
        foreach (GameObject weaponModel in WeaponModels)
        {
            if (weaponModel.name == weaponName)
            {
                Debug.Log(weaponName);
                return weaponModel;
            }
        }
        return null;
    }
}

