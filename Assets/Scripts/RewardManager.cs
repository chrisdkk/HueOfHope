using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class RewardManager : MonoBehaviour
{
    
    [SerializeField] private GameObject rewardPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
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

        // Show them in a popup to the player
        int i = 0;
        foreach (Card card in rewardPrefab.GetComponentsInChildren<Card>())
        {
            card.data = rewards[i];
            i++;
        }
        Instantiate(rewardPrefab, new Vector3(0, 0, -4), Quaternion.identity);
    }
}
