using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public int HealingAmount { get; set; }

    public List<CardData> deck;
    public List<CardData> AllAvailableCards;
    private List<StarterDecks> starterDecks;

    void Awake()
    {
        // init scene one, dontdestroyonload
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MaxActionPoints = 3;
        BurnTickDamage = 4;

        mapSystem.InitializeMapSystem();
        AllAvailableCards = Resources.LoadAll<CardData>("Cards/").ToList();

        if (type == GameType.NewGame)
        {
            maxPlayerHealth = 30;
            HealingAmount = 10;
            CurrentPlayerHealth = maxPlayerHealth;
            tutorialWindow.SetActive(true);
            starterDecks = Resources.LoadAll<StarterDecks>("Decks/").ToList();
            TutorialProgress tutorialProgress = tutorialWindow.GetComponent<TutorialProgress>();
            for (int i = 0; i < tutorialProgress.starterDeckButtons.Count; i++)
            {
                tutorialProgress.starterDeckButtons[i].text = starterDecks[i].name;
            }
        }
        else if (type == GameType.OldGame)
        {
            CurrentPlayerHealth = SaveSystem.Instance.GetSavedPlayerHealth();
            maxPlayerHealth = SaveSystem.Instance.GetSavedMaxPlayerHealth();
            HealingAmount = SaveSystem.Instance.GetSavedHealingAmount();
            mapSystem.currentChapterIndex = SaveSystem.Instance.GetSavedChapterProgress();
            mapSystem.currentStageIndex = SaveSystem.Instance.GetSavedStageProgress();

            deck.AddRange(SaveSystem.Instance.GetSavedPlayerDeck());
        }

        BattleManager.Instance.Initialize(deck, mapSystem.GetEnemies(), mapSystem.GetBackground(),
            mapSystem.GetStory());
    }

    public void SetStarterDeck(int deckID)
    {
        deck.AddRange(starterDecks[deckID].cards);
        BattleManager.Instance.InitializeStarterDeck(deck);
    }

    public static void SetGameType(GameType newType)
    {
        type = newType;
    }
}