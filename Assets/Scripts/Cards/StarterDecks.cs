using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StarterDeck", menuName = "ScriptableObjects/StarterDeck", order = 3)]
public class StarterDecks : ScriptableObject
{
    public List<CardData> cards;
    public string name;
}
