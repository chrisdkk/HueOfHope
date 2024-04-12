using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 30;
    
    [SerializeField]
    private int enemyActionStrength = 6;

    [SerializeField] private GameObject healthBarUI;
    
    public Stats stats = new Stats();

    private EnemyPattern enemyPattern = new EnemyPattern();
    
    // Start is called before the first frame update
    void Start()
    {
        // this should define the enemies order of action during a round
        // example: attack -> block -> attack
        // this could be done on a button click for the prototype and not in Start()
        stats.health = maxHealth;
        enemyPattern.SetPattern();
        UpdateHealthBar();
    }

    public void EnemyAttack()
    {
        PatternTypes pattern = enemyPattern.GetCurrentActionPattern();

        switch (pattern)
        {
            case PatternTypes.Attack:
                GameStateManager.Instance.BattleManager.PlayerStats.health -= enemyActionStrength;
                break;
            case PatternTypes.Block:
                stats.defense += enemyActionStrength;
                break;
            default: break;
        }
    }

    public void UpdateHealthBar()
    {
        healthBarUI.transform.localScale = new Vector3((float)stats.health / (float)maxHealth, 0.04f, 0.5f);
    }
}
