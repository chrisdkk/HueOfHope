using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using Random = System.Random;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private MapSystem mapSystem;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject additionalBackgroundItems;
    [SerializeField] private GameObject chooseRewardVerticalGroup;
    [SerializeField] private GameObject addCardVerticalGroup;
    [SerializeField] private GameObject removeCardVerticalGroup;
    [SerializeField] private GameObject backToSelectionButton;
    [SerializeField] private Button addCardButton;
    [SerializeField] private Button removeCardButton;
    [SerializeField] private GameObject healOption;
    [SerializeField] private GameObject cardParent;

    private RewardCard selectedReward;
    private List<CardData> rewards = new List<CardData>();
    private float fadeDuration = 0.5f;

    public delegate void BattleEndedEventHandler();

    public event BattleEndedEventHandler OnBattleEnd;

    public void Initialize(string storyText)
    {
        additionalBackgroundItems.transform.Find("Story").GetComponent<TextMeshProUGUI>().text = storyText;
        healOption.GetComponentInChildren<TextMeshProUGUI>().text =
            "Heal " + GameInitializer.Instance.HealingAmount + " HP";
    }

    public void StartRewardManager()
    {
        canvasGroup.gameObject.SetActive(true);
        background.SetActive(true);
        additionalBackgroundItems.SetActive(true);
        chooseRewardVerticalGroup.SetActive(true);
        canvasGroup.DOFade(1f, fadeDuration);
        if (mapSystem.chapterList[mapSystem.currentChapterIndex].stageList.Count != mapSystem.currentStageIndex + 1)
        {
            healOption.SetActive(true);
        }
    }

    public void HealPlayer()
    {
        GameInitializer.Instance.CurrentPlayerHealth =
            GameInitializer.Instance.CurrentPlayerHealth + GameInitializer.Instance.HealingAmount >
            GameInitializer.Instance.maxPlayerHealth
                ? GameInitializer.Instance.maxPlayerHealth
                : GameInitializer.Instance.CurrentPlayerHealth + GameInitializer.Instance.HealingAmount;

        addCardButton.interactable = false;
        removeCardButton.interactable = false;
        rewards.RemoveRange(0, rewards.Count);

        background.SetActive(false);
        additionalBackgroundItems.SetActive(false);
        chooseRewardVerticalGroup.SetActive(false);
        backToSelectionButton.SetActive(false);
        healOption.SetActive(false);

        // after rewards have been chosen, invoke
        canvasGroup.DOFade(0f, fadeDuration).OnComplete(() => canvasGroup.gameObject.SetActive(false));
        OnBattleEnd?.Invoke();
    }

    public void ShowAddCard()
    {
        additionalBackgroundItems.SetActive(false);
        chooseRewardVerticalGroup.SetActive(false);
        backToSelectionButton.SetActive(true);
        
        // Choose 3 random card data(s) as reward
        if (rewards.Count == 0)
        {
            Random r = new Random();
            while (rewards.Count < 3)
            {
                int index = r.Next(GameInitializer.Instance.AllAvailableCards.Count);
                // Prevent duplicates
                if (!rewards.Contains(GameInitializer.Instance.AllAvailableCards[index]) &&
                    GameInitializer.Instance.AllAvailableCards[index].cardName != "Kick" &&
                    GameInitializer.Instance.AllAvailableCards[index].cardName != "Cloak Block")
                {
                    rewards.Add(GameInitializer.Instance.AllAvailableCards[index]);
                }
            }
        }

        // Load card data into card game object and register onclick
        for (int i = 0; i < rewards.Count; i++)
        {
            GameObject instCard = cardParent.transform.GetChild(i).gameObject;
            instCard.GetComponent<CardVisual>().LoadCardData(rewards[i]);
            instCard.GetComponent<RewardCard>().OnClick += HandleCardOnClick;
        }

        addCardVerticalGroup.SetActive(true);
    }

    public void ShowRemoveCard()
    {
        additionalBackgroundItems.SetActive(false);
        backToSelectionButton.SetActive(true);
        chooseRewardVerticalGroup.SetActive(false);
        removeCardVerticalGroup.SetActive(true);
        removeCardVerticalGroup.GetComponent<RewardCardListUIController>().OpenDeck(HandleCardOnClick);
    }

    /* Remove the highlighting, if another card was clicked */
    void HandleCardOnClick(RewardCard clickedCard)
    {
        if (selectedReward != null && selectedReward != clickedCard)
        {
            selectedReward.OnOtherRewardChosen();
        }

        selectedReward = clickedCard;
        clickedCard.OnRewardChosen();

        // Enable the button if a card is selected
        addCardButton.interactable = true;
        removeCardButton.interactable = true;
    }

    /* Select the reward, add it to the deck and remove the reward window */
    public void HandleAddCardButtonOnClick()
    {
        if (selectedReward != null)
        {
            FindObjectOfType<AudioManager>().Play("ButtonClick");
            GameInitializer.Instance.deck.Add(selectedReward.GetComponent<CardVisual>().CardData);

            selectedReward.OnOtherRewardChosen();
            selectedReward = null;
            rewards.RemoveRange(0, rewards.Count);

            addCardButton.interactable = false;
            removeCardButton.interactable = false;

            background.SetActive(false);
            addCardVerticalGroup.SetActive(false);
            backToSelectionButton.SetActive(false);
            healOption.SetActive(false);

            // after rewards have been chosen, invoke
            canvasGroup.DOFade(0f, fadeDuration).OnComplete(() => canvasGroup.gameObject.SetActive(false));
            OnBattleEnd?.Invoke();
        }
    }

    public void HandleRemoveCardButtonOnClick()
    {
        if (selectedReward != null)
        {
            FindObjectOfType<AudioManager>().Play("ButtonClick");
            GameInitializer.Instance.deck.Remove(selectedReward.GetComponent<CardVisual>().CardData);

            selectedReward.OnOtherRewardChosen();
            selectedReward = null;
            rewards.RemoveRange(0, rewards.Count);

            addCardButton.interactable = false;
            removeCardButton.interactable = false;

            background.SetActive(false);
            removeCardVerticalGroup.SetActive(false);
            backToSelectionButton.SetActive(false);
            healOption.SetActive(false);
            removeCardVerticalGroup.GetComponent<RewardCardListUIController>().Close();


            // after rewards have been chosen, invoke
            canvasGroup.DOFade(0f, fadeDuration).OnComplete(() => canvasGroup.gameObject.SetActive(false));
            OnBattleEnd?.Invoke();
        }
    }

    public void HandleBackToSelectionButtonOnClick()
    {
        if (addCardVerticalGroup.activeSelf)
        {
            addCardVerticalGroup.SetActive(false);
        }
        else
        {
            removeCardVerticalGroup.SetActive(false);
            removeCardVerticalGroup.GetComponent<RewardCardListUIController>().Close();
        }

        if (selectedReward != null)
        {
            selectedReward.OnOtherRewardChosen();
            selectedReward = null;
            addCardButton.interactable = false;
            removeCardButton.interactable = false;
        }

        chooseRewardVerticalGroup.SetActive(true);
        additionalBackgroundItems.SetActive(true);
        backToSelectionButton.SetActive(false);
    }
}