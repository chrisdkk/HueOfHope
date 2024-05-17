using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using Random = System.Random;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject additionalBackgroundItems;
    [SerializeField] private GameObject healingVerticalGroup;
    [SerializeField] private GameObject chooseRewardVerticalGroup;
    [SerializeField] private GameObject addCardVerticalGroup;
    [SerializeField] private GameObject removeCardVerticalGroup;
    [SerializeField] private GameObject backToSelectionButton;
    [SerializeField] private Button addCardButton;
    [SerializeField] private Button removeCardButton;
    [SerializeField] private GameObject cardParent;
    [SerializeField] private int healingAmount = 0;
    private RewardCard selectedReward;
    private List<CardData> rewards = new List<CardData>();
    private bool backToFirstDecision = true;

    public delegate void BattleEndedEventHandler();

    public event BattleEndedEventHandler OnBattleEnd;

    public void Initialize(string storyText)
    {
        additionalBackgroundItems.transform.Find("Story").GetComponent<TextMeshProUGUI>().text = storyText;
    }

    public void StartRewardManager()
    {
        background.SetActive(true);
        additionalBackgroundItems.SetActive(true);
        healingVerticalGroup.SetActive(true);
    }

    public void HealPlayer()
    {
        GameStateManager.Instance.CurrentPlayerHealth =
            GameStateManager.Instance.CurrentPlayerHealth + healingAmount > GameStateManager.Instance.maxPlayerHealth
                ? GameStateManager.Instance.maxPlayerHealth
                : GameStateManager.Instance.CurrentPlayerHealth + healingAmount;

        addCardButton.interactable = false;
        removeCardButton.interactable = false;

        background.SetActive(false);
        additionalBackgroundItems.SetActive(false);
        healingVerticalGroup.SetActive(false);
        backToSelectionButton.SetActive(false);

        // after rewards have been chosen, invoke
        OnBattleEnd?.Invoke();
    }

    public void ShowRewardSelection()
    {
        healingVerticalGroup.SetActive(false);
        backToSelectionButton.SetActive(true);
        chooseRewardVerticalGroup.SetActive(true);
    }

    public void ShowAddCard()
    {
        additionalBackgroundItems.SetActive(false);
        backToFirstDecision = false;
        // Choose 3 random card datas as reward
        if (rewards.Count == 0)
        {
            Random r = new Random();
            while (rewards.Count < 3)
            {
                int index = r.Next(GameStateManager.Instance.AllAvailableCards.Count);
                // Prevent duplicates
                if (!rewards.Contains(GameStateManager.Instance.AllAvailableCards[index]))
                {
                    rewards.Add(GameStateManager.Instance.AllAvailableCards[index]);
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

        chooseRewardVerticalGroup.SetActive(false);
        addCardVerticalGroup.SetActive(true);
    }

    public void ShowRemoveCard()
    {
        additionalBackgroundItems.SetActive(false);
        backToFirstDecision = false;
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
            GameStateManager.Instance.deck.Add(selectedReward.GetComponent<CardVisual>().CardData);

            selectedReward.OnOtherRewardChosen();
            selectedReward = null;
            rewards.RemoveRange(0, rewards.Count);

            addCardButton.interactable = false;
            removeCardButton.interactable = false;

            background.SetActive(false);
            addCardVerticalGroup.SetActive(false);
            backToSelectionButton.SetActive(false);

            // after rewards have been chosen, invoke
            OnBattleEnd?.Invoke();
        }
    }

    public void HandleRemoveCardButtonOnClick()
    {
        if (selectedReward != null)
        {
            FindObjectOfType<AudioManager>().Play("ButtonClick");
            GameStateManager.Instance.deck.Remove(selectedReward.GetComponent<CardVisual>().CardData);

            selectedReward.OnOtherRewardChosen();
            selectedReward = null;
            rewards.RemoveRange(0, rewards.Count);

            addCardButton.interactable = false;
            removeCardButton.interactable = false;

            background.SetActive(false);
            removeCardVerticalGroup.SetActive(false);
            removeCardVerticalGroup.GetComponent<RewardCardListUIController>().Close();
            backToSelectionButton.SetActive(false);

            // after rewards have been chosen, invoke
            OnBattleEnd?.Invoke();
        }
    }

    public void HandleBackToSelectionButtonOnClick()
    {

        if (backToFirstDecision)
        {
            chooseRewardVerticalGroup.SetActive(false);
            healingVerticalGroup.SetActive(true);
            backToSelectionButton.SetActive(false);
        }
        else
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
            backToFirstDecision = true;
        }
        additionalBackgroundItems.SetActive(true);
    }
}