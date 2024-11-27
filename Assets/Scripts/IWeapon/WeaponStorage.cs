using System.Collections.Generic;
using UnityEngine;

public class WeaponStorage : MonoBehaviour
{
    public static WeaponStorage Instance;
    public List<GameObject> groundWeaponModels;

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

    public GameObject GetWeaponModelByName(string weaponName)
    {
        foreach (GameObject weaponModel in groundWeaponModels)
        {
            if (weaponModel.name == weaponName)
            {
                Debug.Log(weaponModel.name);
                return weaponModel;
            }
        }
        return null;
    }
}

