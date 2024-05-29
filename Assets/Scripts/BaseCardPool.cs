using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCardPool : MonoBehaviour
{
    [SerializeField] protected GameObject cardPrefab;
    [SerializeField] protected int initialSize;
    [SerializeField] protected int stepSize;

    protected readonly Queue<GameObject> pool = new Queue<GameObject>();

    protected void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject instance = Instantiate(cardPrefab, transform);
            pool.Enqueue(instance);
            instance.SetActive(false);
        }
    }

    protected void AddStep()
    {
        for (int i = 0; i < stepSize; i++)
        {
            GameObject instance = Instantiate(cardPrefab, transform);
            pool.Enqueue(instance);
            instance.SetActive(false);
        }
    }

    public GameObject GetCard(CardData data)
    {
        if (pool.Count == 0) AddStep();

        GameObject card = pool.Dequeue();
        card.SetActive(true);
        card.GetComponent<CardVisual>().LoadCardData(data);
        return card;
    }

    public void ReleaseCard(GameObject card)
    {
        pool.Enqueue(card);
        card.SetActive(false);
    }

    public void Clear()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject card = transform.GetChild(i).gameObject;
            pool.Enqueue(card);
            card.SetActive(false);
        }
    }
}