using UnityEngine;

public class HealthMonitor : MonoBehaviour
{
    public Animator animator;


    // Update is called once per frame
    public void UpdatePlayerAnimation(int PlayerHealth)
    {
        // Set the health parameter in the Animator
        animator.SetInteger("PlayerHealth", PlayerHealth);
    }
}
