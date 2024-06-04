using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HandSystem
{
    public enum HandState
    {
        Idle,
        Hover,
        Selected
    }

    public class CardMovementController : MonoBehaviour
    {
        [SerializeField] private LayerMask handLayerMask;
        [SerializeField] private LayerMask targetLayerMask;
        [SerializeField] private float playYBoundary;
        [SerializeField] private Transform playTransform;
        [SerializeField] private GameObject playArrow;

        private HandState handState;
        private GameObject selectedCard;
        private bool useArrowSelection;
        private bool isTargetHovered;
        private Collider targetCollider;
        private EventSystem eventSystem;
        private Camera mainCamera;

        public delegate void CardPlayHandler(GameObject card, GameObject target);

        public event Action OnIdle;
        public event Action<GameObject> OnHover;
        public event Action<CardData> OnSelect;
        public event Action<GameObject> OnHoverTarget;
        public event Action OnLeaveTarget;
        public event CardPlayHandler OnPlay;
        public event Action OnTooLowEnergy;

        private void Start()
        {
            eventSystem = EventSystem.current;
            playArrow.SetActive(false);
            handState = HandState.Idle;
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (BattleManager.Instance.isPaused) return;
            switch (handState)
            {
                case HandState.Idle:
                    HandleIdleState();
                    break;
                case HandState.Hover:
                    HandleHoverState();
                    break;
                case HandState.Selected:
                    HandleSelectedState();
                    if (Input.GetMouseButtonDown(1))
                        TransitionState(HandState.Idle);
                    break;
            }
        }

        private void HandleIdleState()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100, handLayerMask))
            {
                selectedCard = hit.collider.gameObject;
                TransitionState(HandState.Hover);
            }
        }

        private void HandleHoverState()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            bool isHit = Physics.Raycast(ray, out var hit, 100, handLayerMask);
            if (!isHit || hit.collider.gameObject != selectedCard)
            {
                TransitionState(HandState.Idle);
                return;
            }

			if (Input.GetMouseButtonDown(0))
			{
				if (selectedCard.GetComponent<CardVisual>().isEnabled)
					TransitionState(HandState.Selected);
				else
				{
					FindObjectOfType<AudioManager>().Play("NoActionpoint");
					OnTooLowEnergy?.Invoke();
				}
					
			}
		}

        private void HandleSelectedState()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!useArrowSelection)
                {
                    OnPlay?.Invoke(selectedCard, null);
                    TransitionState(HandState.Idle);
                }
                else if (isTargetHovered)
                {
                    OnPlay?.Invoke(selectedCard, targetCollider.gameObject);
                    TransitionState(HandState.Idle);
                }

                return;
            }

            if (useArrowSelection) UpdateArrowSelection();
            else UpdateCardPosition();
        }

        private void UpdateArrowSelection()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100, targetLayerMask))
            {
                if (isTargetHovered) return;
                isTargetHovered = true;
                targetCollider = hit.collider;
                OnHoverTarget?.Invoke(targetCollider.gameObject);
            }
            else if (isTargetHovered)
            {
                isTargetHovered = false;
                targetCollider = null;
                OnLeaveTarget?.Invoke();
            }
        }

        private void UpdateCardPosition()
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 target = new Vector3(mousePos.x, mousePos.y, selectedCard.transform.position.z);
            selectedCard.transform.position =
                Vector3.Lerp(selectedCard.transform.position, target, 20f * Time.deltaTime);
        }

        private void TransitionState(HandState newState)
        {
            var visual = selectedCard != null ? selectedCard.GetComponent<CardVisual>() : null;
            handState = newState;
            switch (newState)
            {
                case HandState.Idle:
                    visual.UpdateColliderSize(false);
                    visual.HideDetails();
                    selectedCard = null;
                    useArrowSelection = false;
                    playArrow.SetActive(false);
                    OnIdle?.Invoke();
                    eventSystem.enabled = true;
                    break;
                case HandState.Hover:
                    visual.ShowDetails();
                    visual.UpdateColliderSize(true);
                    OnHover?.Invoke(selectedCard);
                    break;
                case HandState.Selected:
                    visual.ShowDetails();
                    CardData data = visual.CardData;
                    OnSelect?.Invoke(data);
                    useArrowSelection = data.cardType == CardType.Attack && !data.multiTarget;
                    if (useArrowSelection) selectedCard.transform.DOMove(playTransform.position, 0.2f);
                    playArrow.SetActive(useArrowSelection);
                    eventSystem.enabled = false;
                    break;
            }
        }
    }
}