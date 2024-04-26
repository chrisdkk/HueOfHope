using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private GameObject chooseRewardButton;

    private NonBattleCard selectedReward;
    private Button buttonButtonComponent;

    public delegate void BattleEndedEventHandler();

    public event BattleEndedEventHandler OnBattleEnd;

    void Start()
    {
        buttonButtonComponent = chooseRewardButton.GetComponent<Button>();
    }

    public void ShowReward()
    {
        // choose 3 random cards as reward
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

        // Register for onclick on button
        chooseRewardButton.GetComponent<ChooseRewardButtonUI>().OnClick += HandleButtonOnClick;

        // Load card data into card game object
        for (int i = 0; i < transform.childCount; i++)
        {
            CardVisual cardVisual = transform.GetChild(i).GetComponentInChildren<CardVisual>();
            cardVisual.cardData = rewards[i];
            cardVisual.UpdateCardVisual();
            cardVisual.transform.parent.gameObject.SetActive(true);
        }

        // Register for onclick on cards
        foreach (NonBattleCard nonBattleCard in transform.GetComponentsInChildren<NonBattleCard>())
        {
            nonBattleCard.OnClick += HandleCardOnClick;
        }
    }

    /* Remove the highlighting, if another card was clicked */
    void HandleCardOnClick(NonBattleCard clickedCard)
    {
        if (selectedReward != null && selectedReward != clickedCard)
        {
            selectedReward.OnOtherRewardChosen();
        }

        selectedReward = clickedCard;
        clickedCard.OnRewardChosen();

        // Enable the button if a card is selected
        buttonButtonComponent.interactable = true;
    }

    /* Select the reward, add it to the deck and remove the reward window */
    void HandleButtonOnClick()
    {
        if (selectedReward != null)
        {
            GameStateManager.Instance.deck.Add(selectedReward.GetComponent<CardVisual>().cardData);
            selectedReward.OnOtherRewardChosen();
            selectedReward = null;
            buttonButtonComponent.interactable = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            
            // end current stage once a reward has been chosen
            OnBattleEnd?.Invoke();
        }
    }
}