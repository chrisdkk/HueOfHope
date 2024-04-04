using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 30;
    private int health;
    
    private Stats stats = new Stats();

    private EnemyPattern enemyPattern = new EnemyPattern();
    
    // Start is called before the first frame update
    void Start()
    {
        // this should define the enemies order of action during a round
        // example: attack -> block -> attack
        // this could be done on a button click for the prototype and not in Start()
        enemyPattern.SetPattern();
    }

    // Update is called once per frame
    void Update()
    {
        // receive a pattern of actions for the enemy
        // enemyPattern.GetPattern();
        // execute this when it is the enemies turn during a round
        // enemyPattern.ExecutePatternAction();
    }
}
