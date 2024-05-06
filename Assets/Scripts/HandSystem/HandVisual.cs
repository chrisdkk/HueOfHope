using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace HandSystem
{
	public class HandVisual : MonoBehaviour
	{
		[SerializeField] private HandManager handManager;
		[SerializeField] private CardMovementController stateController;
		[SerializeField] private Transform drawTransform;
		[SerializeField] private Transform discardTransform;

		[Header("Card arrangement variables")]
		[SerializeField] private float fanAngle = 90f;
		[SerializeField] private float horizontalOffset = 0.1f;
		[SerializeField] private float moveDuration;

		[Header("Focused card variables")]
		[SerializeField] private float hoverY = 1f;
		[SerializeField] private float focusScale = 1.5f;

		private readonly List<GameObject> cards = new List<GameObject>();
		private Sequence lastSequence;

		private void Start()
		{
			handManager.OnDrawCard += HandleDraw;
			handManager.OnDiscardCard += HandleDiscard;
			stateController.OnIdle += ArrangeCards;
			stateController.OnHover += HandleHover;
		}

		private void HandleDraw(GameObject card, Action onFinishAnimation)
		{
			card.transform.position = drawTransform.position;
			card.transform.localScale = Vector3.zero;
			cards.Add(card);

			Sequence sequence = DOTween.Sequence();
			if (lastSequence.IsActive()) sequence.Append(lastSequence);
			sequence.Append(card.transform.DOScale(Vector3.one, moveDuration));
			for (int i = 0; i < cards.Count; i++)
			{
				sequence.Join(cards[i].transform.DORotate(CalculateCardRotation(i), moveDuration));
				sequence.Join(cards[i].transform.DOLocalMove(CalculateCardPosition(i), moveDuration));
			}

			sequence.AppendCallback(() => onFinishAnimation?.Invoke());
			sequence.AppendInterval(moveDuration);
			sequence.OnKill(() => lastSequence = null);
			lastSequence = sequence;
		}

		private void HandleDiscard(GameObject card, Action onFinishAnimation)
		{
			cards.Remove(card);

			Sequence sequence = DOTween.Sequence();
			if (lastSequence.IsActive()) sequence.Append(lastSequence);
			sequence.Append(card.transform.DOMove(discardTransform.position, moveDuration));
			sequence.Join(card.transform.DOScale(Vector3.zero, moveDuration));
			for (int i = 0; i < cards.Count; i++)
			{
				if (cards[i] != card)
				{
					sequence.Join(cards[i].transform.DORotate(CalculateCardRotation(i), moveDuration));
					sequence.Join(cards[i].transform.DOLocalMove(CalculateCardPosition(i), moveDuration));
				}
			}

			sequence.AppendCallback(() => { onFinishAnimation?.Invoke(); });
			sequence.OnKill(() => lastSequence = null);
			lastSequence = sequence;
		}

		private void HandleHover(GameObject hoveredCard)
		{
			int hoverID = cards.FindIndex(card => card == hoveredCard);
			if (hoverID == -1) return;

			for (int i = 0; i < cards.Count; i++)
			{
				hoveredCard.transform.DOKill();
				if (i == hoverID) continue;
				Vector3 offset = new Vector3(0.5f / (i - hoverID), 0, 0);
				cards[i].transform.localScale = Vector3.one;
				cards[i].transform.localRotation = Quaternion.Euler(CalculateCardRotation(i));
				cards[i].transform.DOLocalMove(CalculateCardPosition(i) + offset, moveDuration);
			}

			hoveredCard.transform.localScale = Vector3.one * focusScale;
			hoveredCard.transform.localRotation = Quaternion.identity;
			hoveredCard.transform.localPosition = new Vector3(CalculateCardPosition(hoverID).x, hoverY, -2f);
		}

		private void ArrangeCards()
		{
			for (int i = 0; i < cards.Count; i++)
			{
				cards[i].transform.DOScale(Vector3.one, moveDuration);
				cards[i].transform.DORotate(CalculateCardRotation(i), moveDuration);
				cards[i].transform.DOLocalMove(CalculateCardPosition(i), moveDuration);
			}
		}

		private Vector3 CalculateCardPosition(int index)
		{
			// get card position in range(-1,1)
			float normalizedPosition = cards.Count > 1 ? 2f * index / (cards.Count - 1) - 1f : 0f;
			float verticalOffset = Mathf.Tan(Mathf.Deg2Rad * fanAngle) * horizontalOffset * 2f;

			return new Vector3(horizontalOffset * (index - (cards.Count - 1) / 2f),
				verticalOffset * Mathf.Cos(normalizedPosition * Mathf.PI / 2f), -1f - index * 0.01f);
		}

		private Vector3 CalculateCardRotation(int index)
		{
			float normalizedPosition = cards.Count > 1 ? 2f * index / (cards.Count - 1) - 1f : 0f;
			return new Vector3(0, 0, -fanAngle * normalizedPosition);
		}
	}
}