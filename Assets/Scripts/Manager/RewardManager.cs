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
    [SerializeField] private GameObject healingVerticalGroup;
    [SerializeField] private GameObject chooseRewardVerticalGroup;
    [SerializeField] private GameObject addCardVerticalGroup;
    [SerializeField] private GameObject removeCardVerticalGroup;
    [SerializeField] private GameObject backToSelectionButton;
    [SerializeField] private Button addCardButton;
    [SerializeField] private Button removeCardButton;
    [SerializeField] private GameObject cardParent;
    [SerializeField] private int healingAmount=0;
    private RewardCard selectedReward;
    private bool healing;

    public delegate void BattleEndedEventHandler();

    public event BattleEndedEventHandler OnBattleEnd;

    public void Initialize(string storyText, bool receivesHealing)
    {
        chooseRewardVerticalGroup.transform.Find("Story").GetComponent<TextMeshProUGUI>().text = storyText;
        healing = receivesHealing;
    }

    public void StartRewardManager()
    {
        background.SetActive(true);

        if (healing)
        {
            healingVerticalGroup.SetActive(true);
        }
        else
        {
            ShowRewardSelection();
        }
    }

    public void HealPlayer()
    {
        GameStateManager.Instance.CurrentPlayerHealth =
            GameStateManager.Instance.CurrentPlayerHealth + healingAmount > GameStateManager.Instance.maxPlayerHealth
                ? GameStateManager.Instance.maxPlayerHealth
                : GameStateManager.Instance.CurrentPlayerHealth + healingAmount;
        ShowRewardSelection();
    }

    public void ShowRewardSelection()
    {
        if (healing)
        {
            healingVerticalGroup.SetActive(false);
        }
        backToSelectionButton.SetActive(false);
        chooseRewardVerticalGroup.SetActive(true);
    }

    public void ShowAddCard()
    {
        // Choose 3 random card datas as reward
        List<CardData> rewards = new List<CardData>();
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

        // Load card data into card game object
        for (int i = 0; i < rewards.Count; i++)
        {
            CardVisual cardVisual = cardParent.transform.GetChild(i).GetComponentInChildren<CardVisual>();
            cardVisual.LoadCardData(rewards[i]);
        }

        // Register for onclick on cards
        foreach (RewardCard nonBattleCard in cardParent.transform.GetComponentsInChildren<RewardCard>())
        {
            nonBattleCard.OnClick += HandleCardOnClick;
        }

        chooseRewardVerticalGroup.SetActive(false);
        backToSelectionButton.SetActive(true);
        addCardVerticalGroup.SetActive(true);
    }

    public void ShowRemoveCard()
    {
        chooseRewardVerticalGroup.SetActive(false);
        backToSelectionButton.SetActive(true);
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

            addCardButton.interactable = false;
            removeCardButton.interactable = false;

            background.SetActive(false);
            removeCardVerticalGroup.SetActive(false);
            backToSelectionButton.SetActive(false);

            // after rewards have been chosen, invoke
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
        backToSelectionButton.SetActive(false);
        chooseRewardVerticalGroup.SetActive(true);
    }
}