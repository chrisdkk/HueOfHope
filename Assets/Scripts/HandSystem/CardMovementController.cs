using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

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
        private GameObject hoveredCard;
        private bool useArrowSelection;
        private EventSystem eventSystem;

        public delegate void CardIdleEventHandler();

        public delegate void CardFocusEventHandler(GameObject card);

        public delegate void CardPlayHandler(GameObject card, GameObject target);

        public event CardIdleEventHandler OnIdle;
        public event CardFocusEventHandler OnHover;
        public event CardPlayHandler OnPlay;

        private void Start()
        {
            eventSystem = EventSystem.current;
            playArrow.SetActive(false);
            handState = HandState.Idle;
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
                TransitionState(HandState.Hover);
            }
        }

        private void HandleHoverState()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool isHit = Physics.Raycast(ray, out var hit, 100, handLayerMask);
            if (!isHit || hit.collider.gameObject != selectedCard)
            {
                TransitionState(HandState.Idle);
                if (hoveredCard != null)
                {
                    hoveredCard.GetComponent<CardVisual>().ToggleDetails();
                    hoveredCard = null;
                }
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                TransitionState(HandState.Selected);
            }

            // Card is hovered -> Toggle details once and set hoveredCard
            if (hoveredCard == null)
            {
                hit.transform.gameObject.GetComponent<CardVisual>().ToggleDetails();
                hoveredCard = hit.transform.gameObject;
            }
        }

        private void HandleSelectedState()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3 target = new Vector3(mousePos.x, mousePos.y, selectedCard.transform.position.z);
            selectedCard.transform.position =
                Vector3.Lerp(selectedCard.transform.position, target, 20f * Time.deltaTime);

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

            if (!useArrowSelection)
            {
                Vector3 target = new Vector3(mousePos.x, mousePos.y, selectedCard.transform.position.z);
                selectedCard.transform.position =
                    Vector3.Lerp(selectedCard.transform.position, target, 20f * Time.deltaTime);
            }
        }

        private void TransitionState(HandState newState)
        {
            handState = newState;
            switch (newState)
            {
                case HandState.Idle:
                    selectedCard.GetComponent<BoxCollider>().size = new Vector3(1.9f, 2.65f, 0.01f);
                    selectedCard = null;
                    useArrowSelection = false;
                    playArrow.SetActive(false);
                    OnIdle?.Invoke();
                    eventSystem.enabled = true;
                    break;
                case HandState.Hover:
                    selectedCard.GetComponent<BoxCollider>().size = new Vector3(2.2f, 3f, 0.01f);
                    OnHover?.Invoke(selectedCard);
                    break;
                case HandState.PlayArea:
                    CardData data = selectedCard.GetComponent<CardVisual>().CardData;
                    useArrowSelection = data.cardType == CardType.Attack && !data.multiTarget;
                    if (useArrowSelection) selectedCard.transform.DOMove(playTransform.position, 0.2f);
                    playArrow.SetActive(useArrowSelection);
                    eventSystem.enabled = false;
                    break;
                default: break;
            }
        }
    }
}