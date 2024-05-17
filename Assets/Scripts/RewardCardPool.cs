using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardCardPool : MonoBehaviour
{
	[SerializeField] private GameObject cardPrefab;
	[SerializeField] private int initialSize;
	[SerializeField] private int stepSize;

	private readonly Queue<GameObject> pool = new Queue<GameObject>();

	private void Awake()
	{
		for (int i = 0; i < initialSize; i++)
		{
			GameObject instance = Instantiate(cardPrefab, transform);
			pool.Enqueue(instance);
			instance.SetActive(false);
		}
	}

	private void AddStep()
	{
		for (int i = 0; i < stepSize; i++)
		{
			GameObject instance = Instantiate(cardPrefab, transform);
			pool.Enqueue(instance);
			instance.SetActive(false);
		}
	}

	public void Populate(List<CardData> cards, RewardCard.NonBattleCardClickedEventHandler onClickFunction)
	{
		if (cards.Count > pool.Count)
		{
			AddStep();
		}

		for (int i = 0; i < cards.Count; i++)
		{
			GameObject card = pool.Dequeue();
			card.SetActive(true);
			card.GetComponent<CardVisual>().LoadCardData(cards[i]);
			RewardCard rewardCard = card.GetComponent<RewardCard>();
			rewardCard.OnClick+=onClickFunction;
			rewardCard.increasedScale=1.1f;
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