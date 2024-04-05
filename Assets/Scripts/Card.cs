using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Attack,
    Skill,
    Power
}

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class Card : ScriptableObject
{
    public string title;
    public int apCost = 0;
    public CardType cardType = CardType.Attack;
    public bool multiTarget;
    public CardEffect[] effects;
    
    public Stats[] ApplyEffects(Stats[] targets, BattleManager manager)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            targets = effects[i].Apply(targets, manager);
            
        }

        return targets;
    }
}
