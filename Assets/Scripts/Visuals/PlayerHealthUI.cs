using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Character characterScript;
    [SerializeField] private Image healthImage;
    [SerializeField] private TextMeshProUGUI healthNumber;
    private Image healthBar;
   

    void Start()
    {
        characterScript.CharacterStats.OnHealthChange += UpdateHealthBar;

    }
    
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthImage.fillAmount = (float)currentHealth / maxHealth;
        healthNumber.text = currentHealth + "/" + maxHealth;
    }
}