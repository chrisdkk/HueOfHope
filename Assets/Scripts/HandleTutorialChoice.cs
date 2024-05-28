using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleTutorialChoice : MonoBehaviour
{
    public static bool tutorialToggleState;
    
    public void GetChoice()
    {
        tutorialToggleState = gameObject.GetComponent<Toggle>().isOn;
    }
}
