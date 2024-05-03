using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Character characterScript;
    [SerializeField] private Image healthImage;
    [SerializeField] private TextMeshProUGUI healthNumber;
    private Image healthBar;
   

    void Awake()
    {
        characterScript.CharacterStats.OnHealthChange += UpdateHealthBar;

    }
    
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthImage.fillAmount = (float)currentHealth / maxHealth;
        healthNumber.text = currentHealth + "/" + maxHealth;
    }
}