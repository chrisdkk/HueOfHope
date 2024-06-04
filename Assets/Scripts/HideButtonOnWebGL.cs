using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideButtonOnWebGL : MonoBehaviour
{
    void Start()
    {
#if UNITY_WEBGL
gameObject.SetActive(false);
#endif
    }
}

