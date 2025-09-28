using UnityEngine;

public class EnemyHealthBarSprite : MonoBehaviour
{
    [Header("References")]
    public EnemyHealth enemyHealth; 
    
    [Header("Colors")]
    public Color healthyColor = Color.green;
    public Color damagedColor = Color.yellow;
    public Color criticalColor = Color.red;
    
    private SpriteRenderer spriteRenderer;
    
    // Initialize references
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Find EnemyHealth if not assigned
        if (enemyHealth == null)
        {
            enemyHealth = GetComponentInParent<EnemyHealth>();
        }
        
        if (enemyHealth == null)
        {
            Debug.LogError("[EnemyHealthBarSprite] No EnemyHealth found!");
            enabled = false;
            return;
        }
        
        if (spriteRenderer == null)
        {
            Debug.LogError("[EnemyHealthBarSprite] No SpriteRenderer found!");
            enabled = false;
            return;
        }
        
        Debug.Log("[EnemyHealthBarSprite] Initialized for " + enemyHealth.name);
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateHealthBarColor();
    }
    
    // Update the health bar color based on current health percentage
    void UpdateHealthBarColor()
    {
        if (enemyHealth == null || spriteRenderer == null) return;
        
        float healthPercent = enemyHealth.GetHealthPercentage();

        // Change color based on remaining health
        if (healthPercent > 0.6f)
        {
            spriteRenderer.color = healthyColor;
        }
        else if (healthPercent > 0.3f)
        {
            spriteRenderer.color = damagedColor;
        }
        else
        {
            spriteRenderer.color = criticalColor;
        }

        // Hide the bar if the enemy is dead
        if (enemyHealth.IsDead())
        {
            spriteRenderer.color = Color.clear;
        }
    }
}