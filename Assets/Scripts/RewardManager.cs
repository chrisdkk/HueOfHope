using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class RewardManager : MonoBehaviour
{
    
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private GameObject chooseRewardButton;
    private NonBattleCard selectedReward;
    private GameObject instPrefab;

    // Start is called before the first frame update
    void Start()
    {
        ShowReward();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowReward()
    {
        List<CardData> rewards = new List<CardData>();
        Random r = new Random();
        while (rewards.Count < 3)
        {
            int index = r.Next(GameStateManager.Instance.allAvailableCards.Length);
            //if (!rewards.Contains(GameStateManager.AllAvailableCards[index]))
            //{
            rewards.Add(GameStateManager.Instance.allAvailableCards[index]);
            //}
        }

        // Adjust scaling of canvases
        foreach (CanvasScaler canvasScaler in rewardPrefab.GetComponentsInChildren<CanvasScaler>())
        {
            canvasScaler.referenceResolution = new Vector2(1280, 740);
        }

        // Register for onclick on cards
        foreach (NonBattleCard nonBattleCard in rewardPrefab.GetComponentsInChildren<NonBattleCard>())
        {
            nonBattleCard.OnClick += HandleCardOnClick;
        }

        // Register for onclick on button
        chooseRewardButton.GetComponent<ChooseRewardButtonUI>().OnClick += HandleButtonOnClick;
        
        // Load card data into card game object
        int i = 0;
        foreach (CardVisual cardVisual in rewardPrefab.GetComponentsInChildren<CardVisual>())
        {
            cardVisual.Initialize(rewards[i]);
            i++;
        }
        // Show reward window
        instPrefab = Instantiate(rewardPrefab, new Vector3(0, 0, -4), Quaternion.identity);
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

    void HandleButtonOnClick()
    {
        GameStateManager.Instance.deck.Add(selectedReward.data);
        Destroy(instPrefab);
    }
}
