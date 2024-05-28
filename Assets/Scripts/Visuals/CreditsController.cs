using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    private RectTransform scrollerTransform;
    private readonly float upperScrollLimit = 0;

    // Start is called before the first frame update
    void Start()
    {
        scrollerTransform = GetComponent<RectTransform>();

        scrollerTransform.DOLocalMoveY(upperScrollLimit, 10, true).SetEase(Ease.InOutSine);
    }
    
    public void OnQuitToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}