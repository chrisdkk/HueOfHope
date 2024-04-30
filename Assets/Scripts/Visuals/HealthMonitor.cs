using UnityEngine;

public class HealthMonitor : MonoBehaviour
{
    public Animator animator;

void Start()
    {
        BattleManager.Instance.PlayerScript.CharacterStats.OnHealthChange += UpdatePlayerAnimation;

        
    }
    // Update is called once per frame
    public void UpdatePlayerAnimation(int currentHealth, int maxHealth)
    {
        // Set the health parameter in the Animator
        animator.SetInteger("PlayerHealth", currentHealth);
    }
}
