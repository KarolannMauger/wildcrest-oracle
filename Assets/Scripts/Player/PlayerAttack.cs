using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public KeyCode attackKey = KeyCode.E;
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    
    [Header("Audio")]
    public AudioClip attackSfx;
    
    [Header("Visual Feedback")]
    public bool showAttackRange = true;
    
    private AudioSource audioSource;
    private SimplePlayerXP playerXP;
    private float lastAttackTime = -999f;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    
    // Initialize references
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        playerXP = GetComponent<SimplePlayerXP>();
        if (playerXP == null)
        {
            enabled = false;
        }
    }
    
    // Calculate player damage based on level
    int GetPlayerDamage()
    {
        if (playerXP == null) return 1;

        // Damage based on level: Level 0-1 = 1 damage, Level 2-3 = 2 damage, Level 4-5 = 3 damage
        if (playerXP.currentLevel >= 4) return 3;
        if (playerXP.currentLevel >= 2) return 2;
        return 1;
    }

    // Detect enemies in range and handle attack input
    void Update()
    {
        // Detect enemies in range
        DetectEnemiesInRange();

        // Attack if the key is pressed
        if (Input.GetKeyDown(attackKey))
        {
            TryAttack();
        }
    }
    
    // Detect and store enemies within attack range
    void DetectEnemiesInRange()
    {
        enemiesInRange.Clear();

        // Find all colliders within the attack radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        
        foreach (Collider2D hitCollider in hitColliders)
        {
            // Check if it's an enemy
            if (hitCollider.CompareTag("Enemy") && hitCollider.gameObject != gameObject)
            {
                enemiesInRange.Add(hitCollider.gameObject);
            }
        }
    }
    
    void TryAttack()
    {
        // Check cooldown
        if (Time.time - lastAttackTime < attackCooldown)
        {
            return;
        }
        
        if (enemiesInRange.Count == 0)
        {
            return;
        }

        // Attack all enemies in range
        foreach (GameObject enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                AttackEnemy(enemy);
            }
        }

        // Play attack sound
        if (attackSfx && audioSource)
        {
            audioSource.PlayOneShot(attackSfx);
        }
        
        lastAttackTime = Time.time;
    }
    
    // Attack a single enemy by invoking its TakeDamage method
    void AttackEnemy(GameObject enemy)
    {
        // Find EnemyHealth
        var enemyHealth = enemy.GetComponent<MonoBehaviour>();
        if (enemyHealth != null)
        {
            // Try to call TakeDamage via reflection
            var takeDamageMethod = enemyHealth.GetType().GetMethod("TakeDamage");
            if (takeDamageMethod != null)
            {
                int damage = GetPlayerDamage();
                takeDamageMethod.Invoke(enemyHealth, new object[] { damage });
                return;
            }
        }

        // Fallback: find all MonoBehaviour on enemy
        var components = enemy.GetComponents<MonoBehaviour>();
        foreach (var component in components)
        {
            if (component.GetType().Name == "EnemyHealth")
            {
                var takeDamageMethod = component.GetType().GetMethod("TakeDamage");
                if (takeDamageMethod != null)
                {
                    int damage = GetPlayerDamage();
                    takeDamageMethod.Invoke(component, new object[] { damage });
                    return;
                }
            }
        }
    }
    
    // Visualize attack range in the editor
    void OnDrawGizmosSelected()
    {
        if (!showAttackRange) return;

        // Visualize attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Visualize detected enemies
        Gizmos.color = Color.yellow;
        foreach (GameObject enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                Gizmos.DrawLine(transform.position, enemy.transform.position);
            }
        }
    }
}