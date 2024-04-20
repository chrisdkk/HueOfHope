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

    void Start()
    {
        buttonButtonComponent = chooseRewardButton.GetComponent<Button>();
    }
    
    void Update()
    {
        // Enable the button if a card is selected
        if (selectedReward != null)
        {
            buttonButtonComponent.interactable = true;
        }
    }

    public void ShowReward()
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

        // Register for onclick on button
        chooseRewardButton.GetComponent<ChooseRewardButtonUI>().OnClick += HandleButtonOnClick;
        
        // Load card data into card game object
        for(int i=0; i < transform.childCount;i++)
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
        if (!chooseRewardButton.activeSelf)
        {
            chooseRewardButton.SetActive(true);
        }
        
        if (selectedReward!=null && selectedReward != clickedCard)
        {
            selectedReward.OnOtherRewardChosen();
        }
        selectedReward = clickedCard;
        clickedCard.OnRewardChosen();
    }

    /* Select the reward, add it to the deck and remove the reward window */
    void HandleButtonOnClick()
    {
        if (selectedReward != null)
        {
            GameStateManager.Instance.deck.Add(selectedReward.GetComponent<CardVisual>().cardData);
            Destroy(transform.gameObject);
        }
    }
}
