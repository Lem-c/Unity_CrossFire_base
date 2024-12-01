using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStateUI : MonoBehaviour
{
    [SerializeField]
    public Image characterImage;
    public Image healthBarImage;
    public Image lowHealthImage;

    public TextMeshProUGUI ac;
    public TextMeshProUGUI hp;

    public PlayerState playerState;

    private void Start()
    {
        // Check if foregroundImage, backgroundImage, and playerState are properly assigned
        if (characterImage == null || healthBarImage == null ||
            playerState == null || lowHealthImage == null ||
            ac == null || hp == null)
        {
            Debug.LogError("One or more references are null in Update(). Please ensure all are assigned.");
            return;
        }
    }

    void Update()
    {
        float currentHealthRate = playerState.getCurrentHealth() / playerState.maxHealth; ;
        if (currentHealthRate < 0.35)
        {
            lowHealthImage.fillAmount = currentHealthRate;
            characterImage.fillAmount = 0f;
        }
        else {
            lowHealthImage.fillAmount = 0f;
            characterImage.fillAmount = currentHealthRate; 
        }

        // Fetch the health and armor values
        float currentHealth = playerState.getCurrentHealth();
        float currentArmor = playerState.getCurrentArmor();

        // Ensure values don't go below 0
        currentHealth = Mathf.Max(currentHealth, 0);
        currentArmor = Mathf.Max(currentArmor, 0);

        // Update the TextMeshPro UI text fields
        hp.text = "" + currentHealth.ToString("0");
        ac.text = "" + currentArmor.ToString("0");
    }
}

