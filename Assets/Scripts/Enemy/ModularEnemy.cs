using UnityEngine;

public enum EnemyType
{
    Cat,    // Level 0: 3 HP, 1 dégât
    Pig     // Level 4: 10 HP, 3 dégâts
}

public enum MovementType
{
    Static,         // Static
    Horizontal      // Left-Right
}

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class ModularEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public EnemyType enemyType = EnemyType.Cat;
    public MovementType movementType = MovementType.Horizontal;
    
    [Header("Movement - Horizontal")]
    public Transform leftPoint;
    public Transform rightPoint;
    
    [Header("General Movement")]
    public float speed = 2f;
    
    [Header("Combat")]
    public int maxHealth = 3;
    public int touchDamage = 1;
    public float attackCooldown = 1f;
    
    [Header("Player Detection")]
    public Transform player;
    public float behindDetectionRadius = 3f;
    public bool enablePlayerDetection = true;
    
    [Header("Animation")]
    public string attackTrigger = "Attack";
    public float animationSpeed = 0.5f;
    
    [Header("Audio")]
    public AudioClip hitSfx;
    
    // Private variables
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private AudioSource audioSource;
    private EnemyHealth enemyHealth;
    
    private bool movingPositive = true;
    private float lastAttackTime = -999f;
    private int currentHealth;
    
    // Initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Configure enemy based on its type
        ConfigureEnemyByType();

        // Configure animation
        if (animator != null)
        {
            animator.speed = animationSpeed;
        }
        
        currentHealth = maxHealth;
        
        Debug.Log($"[ModularEnemy] {enemyType} initialized - HP: {maxHealth}, Damage: {touchDamage}, Movement: {movementType}");
    }
    
    // Configure enemy stats based on its type
    void ConfigureEnemyByType()
    {
        switch (enemyType)
        {
            case EnemyType.Cat:
                maxHealth = 3;
                touchDamage = 1;
                if (enemyHealth) enemyHealth.maxHealth = maxHealth;
                break;
                
            case EnemyType.Pig:
                maxHealth = 10;
                touchDamage = 3;
                if (enemyHealth) enemyHealth.maxHealth = maxHealth;
                break;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // Player detection behind (if enabled)
        if (enablePlayerDetection && player && movementType != MovementType.Static)
        {
            CheckPlayerBehind();
        }
    }
    
    // Physics update
    void FixedUpdate()
    {
        // Movement based on type
        switch (movementType)
        {
            case MovementType.Static:
                rb.linearVelocity = Vector2.zero;
                break;
                
            case MovementType.Horizontal:
                MoveHorizontal();
                break;
        }
    }
    
    // Move enemy horizontally between leftPoint and rightPoint
    void MoveHorizontal()
    {
        if (!leftPoint || !rightPoint) return;
        
        float dir = movingPositive ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * speed, 0f);
        sr.flipX = !movingPositive;
        
        if (movingPositive && transform.position.x >= rightPoint.position.x)
        {
            movingPositive = false;
        }
        else if (!movingPositive && transform.position.x <= leftPoint.position.x)
        {
            movingPositive = true;
        }
    }
    
    // Check if the player is behind the enemy and turn around if so
    void CheckPlayerBehind()
    {
        Vector2 toPlayer = (Vector2)player.position - (Vector2)transform.position;
        float distanceToPlayer = toPlayer.magnitude;
        
        if (distanceToPlayer <= behindDetectionRadius)
        {
            bool playerOnRight = toPlayer.x > 0;
            bool enemyFacingRight = !sr.flipX;
            
            bool playerBehind = (enemyFacingRight && !playerOnRight) || (!enemyFacingRight && playerOnRight);
            
            if (playerBehind && movementType == MovementType.Horizontal)
            {
                movingPositive = playerOnRight;
                Debug.Log($"[ModularEnemy] {enemyType} detected player behind! Turning around");
            }
        }
    }
    
    // Handle collision with player to inflict damage
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            // Check attack cooldown
            if (Time.time - lastAttackTime < attackCooldown)
                return;
            
            Debug.Log($"[ModularEnemy] {enemyType} attacking player!");

            // Trigger attack animation
            if (animator != null && !string.IsNullOrEmpty(attackTrigger))
            {
                animator.SetTrigger(attackTrigger);
            }

            // The enemy inflicts damage on the player
            var playerHp = col.collider.GetComponent<PlayerHealth>();
            if (playerHp)
            {
                Debug.Log($"[ModularEnemy] {enemyType} inflicts {touchDamage} damage to Player");
                playerHp.TakeDamage(touchDamage);
                if (hitSfx != null)
                {
                    audioSource.PlayOneShot(hitSfx);
                }
            }
            
            lastAttackTime = Time.time;
        }
    }
    // Draw gizmos for detection radius and movement points
    void OnDrawGizmosSelected()
    {
        if (!enablePlayerDetection) return;

        // Player detection zone behind
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, behindDetectionRadius);
        
        // Direction
        Gizmos.color = Color.blue;
        Vector3 direction = Vector3.right;
        if (movementType == MovementType.Horizontal)
        {
            direction = movingPositive ? Vector3.right : Vector3.left;
        }
        Gizmos.DrawRay(transform.position, direction * 1.5f);
        
        // Movement points
        switch (movementType)
        {
            case MovementType.Horizontal:
                if (leftPoint && rightPoint)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(leftPoint.position, rightPoint.position);
                    Gizmos.DrawWireSphere(leftPoint.position, 0.2f);
                    Gizmos.DrawWireSphere(rightPoint.position, 0.2f);
                }
                break;
        }
    }
}