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
        PlayerPrefs.SetInt("PlayerHealth", GameInitializer.Instance.CurrentPlayerHealth);
        PlayerPrefs.SetInt("MaxPlayerHealth", GameInitializer.Instance.maxPlayerHealth);
        PlayerPrefs.SetInt("HealingAmount", GameInitializer.Instance.HealingAmount);

        PlayerPrefs.SetInt("ChapterProgress", mapSystem.currentChapterIndex);
        PlayerPrefs.SetInt("StageProgress", mapSystem.currentStageIndex);

        String deckCardNames = String.Join(",", GameInitializer.Instance.deck.Select(d => d.cardName));
        PlayerPrefs.SetString("PlayerDeck", deckCardNames);

        PlayerPrefs.Save();
        FindObjectOfType<AudioManager>().Play("ButtonClick2");
    }

    public int GetSavedHealingAmount()
    {
        return PlayerPrefs.GetInt("HealingAmount", -1);
    }

    public int GetSavedMaxPlayerHealth()
    {
        return PlayerPrefs.GetInt("MaxPlayerHealth", -1);
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
                SavedDeck.Add(GameInitializer.Instance.AllAvailableCards.Find(cardData => cardData.cardName == name));
            }
        }

        return SavedDeck;
    }
}