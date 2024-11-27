using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateUI : MonoBehaviour
{
    [SerializeField]
    public Image characterImage;
    public Image healthBarImage;
    public Image lowHealthImage;
    public PlayerState playerState;

    private void Start()
    {
        // Check if foregroundImage, backgroundImage, and playerState are properly assigned
        if (characterImage == null || healthBarImage == null ||
            playerState == null || lowHealthImage == null)
        {
            Debug.LogError("One or more references are null in Update(). Please ensure all are assigned.");
            return;
        }
    }

    void Update()
    {
        float currentHealthRate = playerState.getCurrentHealth() / playerState.maxHealth; ;
        if (currentHealthRate < 0.2)
        {
            lowHealthImage.fillAmount = currentHealthRate;
            characterImage.fillAmount = 0f;
        }
        else {
            lowHealthImage.fillAmount = 0f;
            characterImage.fillAmount = currentHealthRate; 
        }
    }
}

