using UnityEngine;

namespace HandSystem
{
	public enum HandState
	{
		Idle,
		Hover,
		Selected,
		PlayArea
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

		public delegate void CardIdleEventHandler();

		public delegate void CardFocusEventHandler(GameObject card);

		public delegate void CardPlayHandler(GameObject card, GameObject target);

		public event CardIdleEventHandler OnIdle;
		public event CardFocusEventHandler OnHover;
		public event CardPlayHandler OnPlay;

		private void Start()
		{
			playArrow.SetActive(false);
			handState = HandState.Idle;
		}

		private void Update()
		{
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
				case HandState.PlayArea:
					HandlePlayAreaState();
					if (Input.GetMouseButtonDown(1))
						TransitionState(HandState.Idle);
					break;
				default: break;
			}
		}

		private void HandleIdleState()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hit, 100, handLayerMask))
			{
				selectedCard = hit.collider.gameObject;
				selectedCard.GetComponent<BoxCollider>().size = new Vector3(2.2f, 3f, 0.01f);
				TransitionState(HandState.Hover);
			}
		}

		private void HandleHoverState()
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			bool isHit = Physics.Raycast(ray, out hit, 100, handLayerMask);
			if (!isHit || hit.collider.gameObject != selectedCard)
			{
				selectedCard.GetComponent<BoxCollider>().size = new Vector3(1.9f, 2.65f, 0.01f);
				TransitionState(HandState.Idle);
				return;
			}

			if (Input.GetMouseButtonDown(0))
			{
				selectedCard.GetComponent<BoxCollider>().size = new Vector3(1.9f, 2.65f, 0.01f);
				TransitionState(HandState.Selected);
			}
		}

		private void HandleSelectedState()
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			selectedCard.transform.position = new Vector3(mousePos.x, mousePos.y, selectedCard.transform.position.z);

			if (selectedCard.transform.position.y > playYBoundary)
				TransitionState(HandState.PlayArea);
		}

		private void HandlePlayAreaState()
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (useArrowSelection)
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					if (Physics.Raycast(ray, out var hit, 100, targetLayerMask))
					{
						OnPlay?.Invoke(selectedCard, hit.transform.gameObject);
						TransitionState(HandState.Idle);
						return;
					}
				}
				else
				{
					OnPlay?.Invoke(selectedCard, null);
					TransitionState(HandState.Idle);
					return;
				}
			}

			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			selectedCard.transform.position = useArrowSelection
				? playTransform.position
				: new Vector3(mousePos.x, mousePos.y, selectedCard.transform.position.z);
		}

		private void TransitionState(HandState newState)
		{
			handState = newState;
			switch (newState)
			{
				case HandState.Idle:
					selectedCard = null;
					useArrowSelection = false;
					playArrow.SetActive(false);
					OnIdle?.Invoke();
					break;
				case HandState.Hover:
					OnHover?.Invoke(selectedCard);
					break;
				case HandState.PlayArea:
					CardData data = selectedCard.GetComponent<CardVisual>().CardData;
					useArrowSelection = data.cardType == CardType.Attack && !data.multiTarget;
					playArrow.SetActive(useArrowSelection);
					break;
				default: break;
			}
		}
	}
}