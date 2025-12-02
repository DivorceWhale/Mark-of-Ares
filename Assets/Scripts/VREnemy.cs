using UnityEngine;
using UnityEngine.AI;

public class VREnemy : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;

    public Animator animator;
    public Weapon weapon;

    private float lastAttackTime;

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            // Always chase the player
            agent.SetDestination(player.position);

            // Update running animation
            animator.SetFloat("Speed", agent.velocity.magnitude);

            // Attack if in range and cooldown passed
            if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // Player not detected → idle
            animator.SetFloat("Speed", 0);
        }
    }

    void Attack()
    {
        // Face the player while attacking
        Vector3 dir = (player.position - transform.position);
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        // Play attack animation
        animator.SetTrigger("Attack");
    }

    // Draw Gizmos to visualize detection and attack ranges
    private void OnDrawGizmosSelected()
    {
        // Detection range (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Attack range (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
