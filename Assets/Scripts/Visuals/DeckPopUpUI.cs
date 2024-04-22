using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckPopUpUI : MonoBehaviour
{
    [SerializeField] private GameObject attackCardContent;
    [SerializeField] private GameObject skillCardContent;

    [SerializeField] private GameObject totalCardCount;
    [SerializeField] private GameObject attackCardCount;
    [SerializeField] private GameObject skillCardCount;

    [HideInInspector] public DeckSystem deckSystem;
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private float cardScale = 80f;

    private bool hasListBeenFilled = false;

    private GameObject deckWindowInstance;

    // you would probably only need one set of positional values
    // but this would allow more control for now (dev phase)
    private float deckAttackCardPosX = 100;
    private float deckAttackCardPosY = -200;

    private float deckSkillCardPosX = 100;
    private float deckSkillCardPosY = -200;

    private int attackCount = 0;
    private int skillCount = 0;

    private void Start()
    {
        PopulateUI();
    }

    private void Update()
    {
    }

    private void PopulateUI()
    {
        // get total amount of card in current deck
        totalCardCount.GetComponent<TextMeshProUGUI>().text = "Deck - Total " + deckSystem.deckList.Count.ToString() + " Cards";

        // deck cards tab
        if (!hasListBeenFilled)
        {
            foreach (CardData card in deckSystem.deckList)
            {
                if (card.cardType == CardType.Attack)
                {
                    // get amount of attack cards
                    attackCount++;
                    attackCardCount.GetComponent<TextMeshProUGUI>().text = attackCount.ToString() + " Cards";

                    cardPrefab.GetComponentInChildren<CardVisual>().cardData = card;

                    GameObject cardInstance = Instantiate(cardPrefab, attackCardContent.transform, false);

                    // cardInstance.GetComponent<NonBattleCard>().OnClick += deckSystem.HandleCardOnClick;

                    cardInstance.transform.localScale = new Vector3(cardScale, cardScale, cardScale);
                    cardInstance.transform.localPosition = new Vector3(deckAttackCardPosX, deckAttackCardPosY, -1f);

                    deckAttackCardPosX += 150;

                    if (deckAttackCardPosX >= 1700)
                    {
                        deckAttackCardPosX = 100;
                        deckAttackCardPosY -= 200;
                    }
                }

                if (card.cardType == CardType.Skill)
                {
                    // get amount of skill cards
                    skillCount++;
                    skillCardCount.GetComponent<TextMeshProUGUI>().text = skillCount.ToString() + " Cards";

                    cardPrefab.GetComponentInChildren<CardVisual>().cardData = card;

                    GameObject cardInstance = Instantiate(cardPrefab, skillCardContent.transform, false);

                    // cardInstance.GetComponent<NonBattleCard>().OnClick += deckSystem.HandleCardOnClick;

                    cardInstance.transform.localScale = new Vector3(cardScale, cardScale, cardScale);
                    cardInstance.transform.localPosition = new Vector3(deckSkillCardPosX, deckSkillCardPosY, -1f);

                    deckSkillCardPosX += 150;

                    if (deckSkillCardPosX >= 1700)
                    {
                        deckSkillCardPosX = 100;
                        deckSkillCardPosY -= 200;
                    }
                }
            }
            
            // prevent filling the ui multiple times causing duplicates
            hasListBeenFilled = true;
        }
    }
}