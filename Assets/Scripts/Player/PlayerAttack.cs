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
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        playerXP = GetComponent<SimplePlayerXP>();
        if (playerXP == null)
        {
            Debug.LogError("[PlayerAttack] SimplePlayerXP not found!");
        }
    }
    
    int GetPlayerDamage()
    {
        if (playerXP == null) return 1;
        
        // Dégâts basés sur le niveau : Level 0-1 = 1 dégât, Level 2-3 = 2 dégâts, Level 4-5 = 3 dégâts
        if (playerXP.currentLevel >= 4) return 3;
        if (playerXP.currentLevel >= 2) return 2;
        return 1;
    }
    
    void Update()
    {
        // Détecter les ennemis à portée
        DetectEnemiesInRange();
        
        // Attaquer si la touche est pressée
        if (Input.GetKeyDown(attackKey))
        {
            TryAttack();
        }
    }
    
    void DetectEnemiesInRange()
    {
        enemiesInRange.Clear();
        
        // Trouver tous les colliders dans le rayon d'attaque
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        
        foreach (Collider2D hitCollider in hitColliders)
        {
            // Vérifier si c'est un ennemi
            if (hitCollider.CompareTag("Enemy") && hitCollider.gameObject != gameObject)
            {
                enemiesInRange.Add(hitCollider.gameObject);
            }
        }
    }
    
    void TryAttack()
    {
        // Vérifier le cooldown
        if (Time.time - lastAttackTime < attackCooldown)
        {
            Debug.Log("[PlayerAttack] Attack on cooldown");
            return;
        }
        
        if (enemiesInRange.Count == 0)
        {
            Debug.Log("[PlayerAttack] No enemies in range");
            return;
        }
        
        Debug.Log($"[PlayerAttack] Attacking {enemiesInRange.Count} enemy(ies)");
        
        // Attaquer tous les ennemis à portée
        foreach (GameObject enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                AttackEnemy(enemy);
            }
        }
        
        // Jouer le son d'attaque
        if (attackSfx && audioSource)
        {
            audioSource.PlayOneShot(attackSfx);
        }
        
        lastAttackTime = Time.time;
    }
    
    void AttackEnemy(GameObject enemy)
    {
        Debug.Log($"[PlayerAttack] Attacking enemy: {enemy.name}");
        
        // Chercher EnemyHealth
        var enemyHealth = enemy.GetComponent<MonoBehaviour>();
        if (enemyHealth != null)
        {
            // Essayer d'appeler TakeDamage via réflection
            var takeDamageMethod = enemyHealth.GetType().GetMethod("TakeDamage");
            if (takeDamageMethod != null)
            {
                int damage = GetPlayerDamage();
                takeDamageMethod.Invoke(enemyHealth, new object[] { damage });
                Debug.Log($"[PlayerAttack] Dealt {damage} damage to {enemy.name} (Player Level {(playerXP != null ? playerXP.currentLevel : 0)})");
                return;
            }
        }
        
        // Fallback: chercher tous les MonoBehaviour sur l'ennemi
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
                    Debug.Log($"[PlayerAttack] Dealt {damage} damage to {enemy.name} (Player Level {(playerXP != null ? playerXP.currentLevel : 0)})");
                    return;
                }
            }
        }
        
        Debug.LogWarning($"[PlayerAttack] No TakeDamage method found on {enemy.name}");
    }
    
    void OnDrawGizmosSelected()
    {
        if (!showAttackRange) return;
        
        // Visualiser la portée d'attaque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Visualiser les ennemis détectés
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