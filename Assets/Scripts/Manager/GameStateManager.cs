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
                deck.AddRange(GetStarterDeck());
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

    private List<CardData> GetStarterDeck()
    {
        List<CardData> starterDeck = new List<CardData>();

        // add available cards to deck
        for (int i = 0; i < 5; i++)
        {
            starterDeck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Cloak Block"));
            starterDeck.Add(AllAvailableCards.Find(cardData => cardData.cardName == "Kick"));
        }

        starterDeck.AddRange(AllAvailableCards.FindAll(cardData =>
            cardData.cardName != "Cloak Block" && cardData.cardName != "Kick"));

        return starterDeck;
    }

    public static void SetGameType(GameType newType)
    {
        type = newType;
    }
}