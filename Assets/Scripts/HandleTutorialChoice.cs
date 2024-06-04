using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleTutorialChoice : MonoBehaviour
{
    public static bool tutorialToggleState=true;
    
    public void SetChoice()
    {
        tutorialToggleState = gameObject.GetComponent<Toggle>().isOn;
    }
}
