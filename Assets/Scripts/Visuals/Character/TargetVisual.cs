using System;
using DG.Tweening;
using HandSystem;
using UnityEngine;
using UnityEngine.Serialization;

public static class DOTweenExtensions
{
	// Extension method to animate the size property of a SpriteRenderer
	public static Tweener DOSize(this SpriteRenderer spriteRenderer, Vector2 endValue, float duration)
	{
		// Use DOTween's To method to create a custom tween for the size property
		return DOTween.To(() => spriteRenderer.size, x => spriteRenderer.size = x, endValue, duration);
	}
}

namespace Visuals.Character
{
	public class TargetVisual : MonoBehaviour
	{
		[SerializeField] private GameObject character; 
		[SerializeField] private SpriteRenderer targetSprite;

		[Header("Animation fields")]
		[SerializeField] private float amplitude = 0.5f;
		[SerializeField] private float duration = 1f;

		private Tweener tweener;
		private Vector2 baseSize;
		private CardMovementController cardMovementController;

		private void Start()
		{
			cardMovementController = FindObjectOfType<CardMovementController>();
			baseSize = targetSprite.size;
			targetSprite.gameObject.SetActive(false);
			cardMovementController.OnIdle += HandleDeselect;
			cardMovementController.OnSelect += HandleCardSelect;
			cardMovementController.OnHoverTarget += HandleHoverTarget;
			cardMovementController.OnLeaveTarget += HandleLeaveTarget;
		}

		private void OnDestroy()
		{
			cardMovementController.OnIdle -= HandleDeselect;
			cardMovementController.OnSelect -= HandleCardSelect;
			cardMovementController.OnHoverTarget -= HandleHoverTarget;
			cardMovementController.OnLeaveTarget -= HandleLeaveTarget;
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
			if (targetSprite.gameObject.activeSelf) return;
			targetSprite.gameObject.SetActive(true);
			tweener = targetSprite.DOSize(baseSize + Vector2.one * amplitude, duration / 2)
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo);
		}

		private void StopAnimation()
		{
			if (!targetSprite.gameObject.activeSelf) return;
			tweener.Kill();
			targetSprite.size = baseSize;
			targetSprite.gameObject.SetActive(false);
		}
	}
}