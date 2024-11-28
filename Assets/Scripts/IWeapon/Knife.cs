using UnityEngine;

[CreateAssetMenu(fileName = "NewKnife", menuName = "Weapon/Knife")]
public class Knife : Weapon
{
    [Range(0f, 20f)]
    public float bladeLength;

    public int lightAttackDamage;
    public int heavyAttackDamage;
}
