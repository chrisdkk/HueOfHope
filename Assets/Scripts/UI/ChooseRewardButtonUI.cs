using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseRewardButtonUI : MonoBehaviour
{
    public delegate void ChooseRewardClickedEventHandler();

    public event ChooseRewardClickedEventHandler OnClick;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnButtonClick()
    {
        OnClick?.Invoke();
    }
}
