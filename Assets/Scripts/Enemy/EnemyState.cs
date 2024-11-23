using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : PlayerState
{
    public override void TakeDamage(float damage, bool isLog=false)
    {
        base.TakeDamage(damage, isLog);
        if (currentHealth <= 0)
        {
            PlayerKillCount.Instance.EnemyDestroyed();
        }
    }
}
