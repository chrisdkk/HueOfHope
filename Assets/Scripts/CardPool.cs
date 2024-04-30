using System;
using System.Collections.Generic;
using UnityEngine;

public class CardPool: MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int initialSize;
    [SerializeField] private int stepSize;

    private List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject instance = Instantiate(cardPrefab, transform);
            pool.Add(instance);
            instance.SetActive(false);
        }
    }

    private void AddStep()
    {
        for (int i = 0; i < stepSize; i++)
        {
            GameObject instance = Instantiate(cardPrefab, transform);
            pool.Add(instance);
            instance.SetActive(false);
        } 
    }

    public void Populate(List<CardData> cards)
    {
        if (cards.Count > pool.Count)
        {
            AddStep();
        }

        for (int i = 0; i < cards.Count; i++)
        {
            pool[i].SetActive(true);
            pool[i].GetComponent<CardVisual>().LoadCardData(cards[i]);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            pool[i].SetActive(false);
        }
    }
}