using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialProgress : MonoBehaviour
{
    [SerializeField] private GameObject drawPileTooltip;
    [SerializeField] private GameObject discardPileTooltip;
    [SerializeField] private GameObject handTooltip;
    [SerializeField] private GameObject actionPointsTooltip;
    [SerializeField] private GameObject enemyActionTooltip;
    [SerializeField] private Image background;

    public void CheckForTutorial()
    {
        if (HandleTutorialChoice.tutorialToggleState)
        {
            drawPileTooltip.SetActive(true);
            discardPileTooltip.SetActive(true);
            handTooltip.SetActive(true);
        }
        else
        {
            background.DOFade(0f, 1).OnComplete(() =>
            {
                gameObject.SetActive(false);
                BattleManager.Instance.Resume();
            });
        }
    }

    public void ReduceBackgroundTransparency()
    {
        background.DOFade(0.1f, 1);
    }

    public void CheckForSecondTutorialPhase()
    {
        if (drawPileTooltip.activeSelf || discardPileTooltip.activeSelf || handTooltip.activeSelf)
            return;

        ShowSecondTutorialPhase();
    }

    private void ShowSecondTutorialPhase()
    {
        enemyActionTooltip.SetActive(true);
        actionPointsTooltip.SetActive(true);
    }

    public void CheckForTutorialEnd()
    {
        if (enemyActionTooltip.activeSelf || actionPointsTooltip.activeSelf)
            return;
        background.DOFade(0f, 1).OnComplete(() =>
        {
            gameObject.SetActive(false);
            BattleManager.Instance.Resume();
        });
    }
}