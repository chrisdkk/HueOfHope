using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDeckButtonUI : MonoBehaviour
{
    public GameObject deckWindow;

    public void OnButtonClick()
    {
        Destroy(deckWindow);
    }
}

