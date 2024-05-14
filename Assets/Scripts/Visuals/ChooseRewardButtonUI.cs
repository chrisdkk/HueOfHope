using System;
using UnityEngine;

public class ChooseRewardButtonUI : MonoBehaviour
{
    public event Action OnClick;

    public void OnButtonClick()
    {
        OnClick?.Invoke();
        FindObjectOfType<AudioManager>().Play("ButtonClick");
    }
}
