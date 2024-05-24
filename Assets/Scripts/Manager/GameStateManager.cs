using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameType
{
    NewGame,
    OldGame
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [SerializeField] private MapSystem mapSystem;
    [SerializeField] private GameObject tutorialWindow;
    private static GameType type;

    public int CurrentPlayerHealth { get; set; }
    public int maxPlayerHealth;
    public int MaxActionPoints { get; set; }
    public int BurnTickDamage { get; set; }

    public List<CardData> deck;
    public List<CardData> AllAvailableCards;

    // Start is called before the first frame update
    void Awake()
    {
        // Initialize 1 GameStateManager per game
        if (Instance == null)
        {
            Instance = this;

            maxPlayerHealth = 30;
            MaxActionPoints = 3;
            BurnTickDamage = 4;

            mapSystem.InitializeMapSystem();
            AllAvailableCards = Resources.LoadAll<CardData>("Cards/").ToList();

            if (type == GameType.NewGame)
            {
                CurrentPlayerHealth = maxPlayerHealth;
                tutorialWindow.SetActive(true);
            }
            else if (type == GameType.OldGame)
            {
                CurrentPlayerHealth = SaveSystem.Instance.GetSavedPlayerHealth();

                mapSystem.currentChapterIndex = SaveSystem.Instance.GetSavedChapterProgress();
                mapSystem.currentStageIndex = SaveSystem.Instance.GetSavedStageProgress();

                deck.AddRange(SaveSystem.Instance.GetSavedPlayerDeck());
            }

            BattleManager.Instance.Initialize(deck, mapSystem.GetEnemies(), mapSystem.GetBackground(),
                mapSystem.GetStory());
        }
    }

    public void SetStarterDeck(int deckID) // Pyromancer=0, Sage=1, Knight=2
    {
        for (int i = 0; i < 5; i++)
        {
            deck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Cloak Block"));
            deck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Kick"));
        }

        AllAvailableCards.Remove(AllAvailableCards.Find(cardData => cardData.cardName == "Kick"));
        AllAvailableCards.Remove(AllAvailableCards.Find(cardData => cardData.cardName == "Cloak Block"));


        switch (deckID)
        {
            case 0:
                deck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Fire Storm"));
                deck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Searing Strike"));
                deck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Scorch"));
                break;
            case 1:
                deck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Calm Drop"));
                deck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Insightful Strike"));
                deck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Flaming Insight"));
                break;
            case 2:
                deck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Barrier Strike"));
                deck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Barrier Strike"));
                deck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Smoldering Shield"));
                break;
        }

        BattleManager.Instance.InitializeStarterDeck(deck);
    }

    public static void SetGameType(GameType newType)
    {
        type = newType;
    }
}