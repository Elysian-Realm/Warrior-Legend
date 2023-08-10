using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
    Character curruntCharacter;
    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;
    private bool isRecovering;

    private void Update()
    {
        if (healthImage.fillAmount < healthDelayImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime;
        }

        if (isRecovering)
        {
            float persentage = curruntCharacter.currentPower / curruntCharacter.maxPower;
            powerImage.fillAmount = persentage;

            if (persentage >= 1)
            {
                isRecovering = false;
            }
        }
    }

    public void OnHealthChange(float persentage)
    {
        healthDelayImage.fillAmount = healthImage.fillAmount;
        healthImage.fillAmount = persentage;
    }

    public void OnPowerChange(Character character)
    {
        isRecovering = true;
        curruntCharacter = character;
    }
}
