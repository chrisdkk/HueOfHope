using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class ViewDeckButtonUI : MonoBehaviour
{
    [SerializeField] private GameObject deckWindow;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Camera camera;

    public void OnButtonClick()
    {
        GameObject instDeckWindow = Instantiate(deckWindow, new Vector3(0, 0, 0), Quaternion.identity);
        
        instDeckWindow.GetComponentInChildren<Canvas>().planeDistance = 3;
        instDeckWindow.transform.GetComponentInChildren<Canvas>().worldCamera = camera;
        Transform contentPanel = instDeckWindow.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).transform;
        float cardPosX = 100;
        float cardPosY = -100;

        // Add cards to scroll view
        foreach (CardData card in GameStateManager.Instance.deck)
        {
            cardPrefab.GetComponentInChildren<Card>().data = card;
            GameObject instCard = Instantiate(cardPrefab, contentPanel, false);
            instCard.transform.localScale = new Vector3(50, 50, 50);
            instCard.transform.localPosition = new Vector3(cardPosX, cardPosY, -0.1f);
            cardPosX += 100;
            if (cardPosX >= 800)
            {
                cardPosX = 100;
                cardPosY -= 150;
            }
        }

        // Adjust content height of scroll view
        RectTransform rectTransform = contentPanel.GetComponent<RectTransform>();
        cardPosY = (cardPosY - 100) * -1;
        rectTransform.sizeDelta = new Vector2(rectTransform.rect.size.x, cardPosY);

        // Set for close UI button
        instDeckWindow.GetComponentInChildren<CloseDeckButtonUI>().deckWindow = instDeckWindow;
    }
}