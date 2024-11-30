using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyState : PlayerState
{
    public bool isAct = true;

    public override void TakeDamage(float damage, bool isLog=false)
    {
        base.TakeDamage(damage, isLog);

        if (currentHealth <= 0)
        {
            isAct = false;
            PlayerKillCount.Instance.EnemyDestroyed();
        }
    }
}
