using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialProgress : MonoBehaviour
{
    [SerializeField] private GameObject drawPileTooltip;
    [SerializeField] private GameObject discardPileTooltip;
    [SerializeField] private GameObject handTooltip;
    [SerializeField] private GameObject actionPointsTooltip;
    [SerializeField] private GameObject enemyActionTooltip;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private Image background;
    [SerializeField] public List<TextMeshProUGUI> starterDeckButtons;
    private bool tutorialEnd = false;

    public void CheckForTutorial()
    {
        if (HandleTutorialChoice.tutorialToggleState)
        {
            drawPileTooltip.SetActive(true);
            discardPileTooltip.SetActive(true);
            handTooltip.SetActive(true);
            continueButton.SetActive(true);
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
        background.DOFade(0.7f, 1);
    }

    public void ContinueTutorial()
    {
        if (tutorialEnd)
        {
            continueButton.SetActive(false);
            enemyActionTooltip.SetActive(false);
            actionPointsTooltip.SetActive(false);
            background.DOFade(0f, 1).OnComplete(() =>
            {
                gameObject.SetActive(false);
                BattleManager.Instance.Resume();
            });
        }
        else
        {
            drawPileTooltip.SetActive(false);
            discardPileTooltip.SetActive(false);
            handTooltip.SetActive(false);
            enemyActionTooltip.SetActive(true);
            actionPointsTooltip.SetActive(true);
            tutorialEnd = true;
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "End tutorial";
        }
    }
}