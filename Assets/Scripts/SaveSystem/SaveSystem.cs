using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;

    [SerializeField] private MapSystem mapSystem;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("PlayerHealth", GameStateManager.Instance.CurrentPlayerHealth);

        PlayerPrefs.SetInt("ChapterProgress", mapSystem.currentChapterIndex);
        PlayerPrefs.SetInt("StageProgress", mapSystem.currentStageIndex);

        String deckCardNames = String.Join(",", GameStateManager.Instance.deck.Select(d => d.cardName));
        PlayerPrefs.SetString("PlayerDeck", deckCardNames);

        PlayerPrefs.Save();
    }

    public int GetSavedPlayerHealth()
    {
        return PlayerPrefs.GetInt("PlayerHealth", -1);
    }

    public int GetSavedChapterProgress()
    {
        return PlayerPrefs.GetInt("ChapterProgress", -1);
    }

    public int GetSavedStageProgress()
    {
        return PlayerPrefs.GetInt("StageProgress", -1);
    }

    public List<CardData> GetSavedPlayerDeck()
    {
        List<CardData> SavedDeck = new List<CardData>();

        String PlayerDeck = PlayerPrefs.GetString("PlayerDeck", "-1");
        String[] CardNames = PlayerDeck.Split(new[] { ',' });

        foreach (String name in CardNames)
        {
            if (name != null)
            {
                SavedDeck.Add(GameStateManager.Instance.AllAvailableCards.Find(cardData => cardData.cardName == name));
            }
        }

        return SavedDeck;
    }
}