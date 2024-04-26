using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public enum CardState {
    Idle,
    Hover,
    Dragging,
    Play,
    Played
}

public enum CardPlayType
{
    FreeMove,
    Arrow
}

public class CardMovement : MonoBehaviour
{
    private static CardMovement selectedCard;
    
    private CardPlayType playType;
    private Vector3 originalLocalPointerPosition;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private CardState currentState = 0;
    private Camera mainCamera;
    private float timeSincePlayed = 0f;
    private Vector3 originPlayedPosition;

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private float playedScale = 0.9f;
    [SerializeField] private Vector2 cardPlayBorder;
    [SerializeField] private Vector3 playPosition;
    [SerializeField] private Vector3 playedPosition;
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private GameObject playArrow;

    // Event for handling a play attempt
    public delegate void PlayEventHandler(GameObject playedCard, [CanBeNull] GameObject enemy);

    public event PlayEventHandler OnPlay;

    public void Initialize(CardData card)
    {
        if (card.cardType == CardType.Attack && !card.multiTarget)
        {
            playType = CardPlayType.Arrow;
        }
        else
        {
            playType = CardPlayType.FreeMove;
        }
    }

    private void Awake()
    {
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;
        originalRotation = transform.localRotation;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        switch (currentState)
        {
            case CardState.Hover:
                HandleHoverState();
                break;
            case CardState.Dragging:
                HandleDragState();
                break;
            case CardState.Play:
                HandlePlayState();
                break;
            case CardState.Played:
                HandlePlayedState();
                break;
        }
    }

    private void TransitionToIdleState()
    {
        currentState = CardState.Idle;
        transform.localScale = originalScale;
        transform.localRotation = originalRotation;
        transform.localPosition = originalPosition;
        glowEffect.SetActive(false);
        playArrow.SetActive(false);
    }

    private void OnMouseEnter()
    {
        if (currentState == CardState.Idle && selectedCard == null)
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
            originalScale = transform.localScale;
            currentState = CardState.Hover;
        }
    }

    private void OnMouseExit()
    {
        if (currentState == CardState.Hover)
        {
            TransitionToIdleState();
        }
    }

    private void OnMouseDown()
    {
        if (currentState == CardState.Hover)
        {
            currentState = CardState.Dragging;
            selectedCard = this;
        } 
    }

    private void HandleHoverState()
    {
        glowEffect.SetActive(true);
        transform.localScale = originalScale * selectScale;
        RectTransform rectTransform = GetComponentInChildren<RectTransform>();
        transform.localPosition = new Vector3(originalPosition.x, (rectTransform.rect.height*rectTransform.transform.localScale.y)/4+.1f, -1);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private void HandleDragState()
    {
        // Set the card's rotation to zero
        transform.localRotation = Quaternion.identity;
        
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldMousePos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 5f));

        transform.localPosition = transform.parent.InverseTransformPoint(worldMousePos);
        
        if (transform.localPosition.y > cardPlayBorder.y)
        {
            currentState = CardState.Play;
            if (playType == CardPlayType.Arrow)
                playArrow.SetActive(true);
        }
    }

    private void HandlePlayState()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldMousePos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 5f));
        Vector3 localMousePos = transform.parent.InverseTransformPoint(worldMousePos);
                 
        if (playType == CardPlayType.Arrow)
        {
            transform.localPosition = playPosition;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            transform.localPosition = localMousePos;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (playType == CardPlayType.Arrow)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 100, layerMask))
                {
                    if (BattleManager.Instance.PlayerScript.CurrentActionPoints >= GetComponent<CardVisual>().cardData.apCost)
                    {
                        selectedCard = null;
                        OnPlay?.Invoke(gameObject, hit.transform.gameObject);
                        currentState = CardState.Played;
                        originPlayedPosition = playPosition;   
                    }
                } 
            }
            else
            {
                if (transform.localPosition.y > cardPlayBorder.y && BattleManager.Instance.PlayerScript.CurrentActionPoints >= GetComponent<CardVisual>().cardData.apCost)
                {
                    selectedCard = null;
                    OnPlay?.Invoke(gameObject, null);
                    currentState = CardState.Played;
                    originPlayedPosition = transform.position;
                }
            }
        }

        if (Input.GetMouseButtonDown(1) || localMousePos.y < -0.2)
        {
            playArrow.SetActive(false);
            selectedCard = null;
            TransitionToIdleState();
        }
    }

    private void HandlePlayedState()
    {
        timeSincePlayed += Time.deltaTime*6;
        playArrow.SetActive(false);
        selectedCard = null;
        transform.localScale = originalScale * playedScale;
        transform.localPosition = Vector3.Lerp(originPlayedPosition, playedPosition, timeSincePlayed);
        transform.localRotation = Quaternion.identity;
    }
}
