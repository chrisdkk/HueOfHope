using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerBlockUI : MonoBehaviour
{
     [SerializeField] private Character characterScript;
     [SerializeField] private Image blockImage;
     [SerializeField] private TextMeshProUGUI blockText;

   void Awake()
   {
        characterScript.CharacterStats.OnStatChange += UpdateBlock;
   }

    // Update is called once per frame
    public void UpdateBlock()
    {
         blockText.text = characterScript.CharacterStats.Block.ToString();
         
        if (characterScript.CharacterStats.Block<=0){
            blockImage.gameObject.SetActive(false);
            blockText.gameObject.SetActive(false);
        }else{
             blockImage.gameObject.SetActive(true);
            blockText.gameObject.SetActive(true);
        }

    }
}
