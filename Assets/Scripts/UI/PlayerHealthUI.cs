using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateUI : MonoBehaviour
{
    [SerializeField]
    public Image healthBarImage;
    public PlayerState playerState;

    void Update()
    {
        healthBarImage.fillAmount = playerState.getCurrentHealth() / playerState.maxHealth;
    }
}

