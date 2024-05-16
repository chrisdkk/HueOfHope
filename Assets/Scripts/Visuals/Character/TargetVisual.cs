using System;
using DG.Tweening;
using HandSystem;
using UnityEngine;

namespace Visuals.Character
{
	public class TargetVisual : MonoBehaviour
	{
		[SerializeField] private GameObject character;
		[SerializeField] private GameObject targetVisual;

		[Header("Animation fields")]
		[SerializeField] private float amplitude = 0.5f;
		[SerializeField] private float duration = 1f;

		private Tweener tweener;
		private CardMovementController cardMovementController;

		private void Start()
		{
			cardMovementController = FindObjectOfType<CardMovementController>();
			targetVisual.SetActive(false);
			cardMovementController.OnIdle += HandleDeselect;
			cardMovementController.OnSelect += HandleCardSelect;
			cardMovementController.OnHoverTarget += HandleHoverTarget;
			cardMovementController.OnLeaveTarget += HandleLeaveTarget;
		}

		private void HandleDeselect()
		{
			StopAnimation();
		}

		private void HandleCardSelect(CardData data)
		{
			if (data.multiTarget && character.CompareTag("Enemy"))
			{
				StartAnimation();
			}
			else if (data.cardType != CardType.Attack && character.CompareTag("Player"))
			{
				StartAnimation();
			}
		}

		private void HandleHoverTarget(GameObject target)
		{
			if (target == character)
			{
				StartAnimation();
			}
		}

		private void HandleLeaveTarget()
		{
			StopAnimation();
		}

		private void StartAnimation()
		{
			if (targetVisual.activeSelf) return;
			targetVisual.SetActive(true);
			tweener = transform.DOLocalMoveY(targetVisual.transform.localPosition.y + amplitude, duration / 2)
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo);
		}

		private void StopAnimation()
		{
			if (!targetVisual.activeSelf) return;
			tweener.Kill();
			targetVisual.transform.localPosition = Vector3.zero;
			targetVisual.SetActive(false);
		}
	}
}