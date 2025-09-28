using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("PV")]
    public int maxHealth = 3;
    public int CurrentHealth { get; private set; }

    [Header("Death")]
    public bool destroyOnDeath = true;
    public float deathDelay = 0.5f;
    public string deathTriggerName = "Death";

    [Header("Damage Effects")]
    public float shakeDuration = 0.3f;
    public float shakeIntensity = 0.1f;
    public Color damageColor = Color.red;
    public float colorFlashDuration = 0.2f;

    [Header("XP Reward")]
    public int xpReward = 1;
    
    [Header("Audio")]
    public AudioClip takeDamageSfx;
    public AudioClip deathSfx;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isDead = false;
    private Vector3 originalPosition;
    private Color originalColor;
    private Coroutine shakeCoroutine;
    private Coroutine colorFlashCoroutine;

    // Initialize components and variables
    void Awake()
    {
        CurrentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        originalPosition = transform.position;
        if (spriteRenderer)
            originalColor = spriteRenderer.color;

        Debug.Log($"[EnemyHealth] Init HP = {CurrentHealth}/{maxHealth}");
    }

    // Function to apply damage to the enemy
    public void TakeDamage(int dmg)
    {
        if (isDead || dmg <= 0) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - dmg);
        Debug.Log($"[EnemyHealth] -{dmg} HP → {CurrentHealth}/{maxHealth}");

        // Damage effects
        PlayDamageEffects();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    // Play visual and audio effects when enemy taking damage
    private void PlayDamageEffects()
    {
        // Damage sound effect
        if (takeDamageSfx && audioSource)
        {
            audioSource.PlayOneShot(takeDamageSfx);
        }

        // Shake effect
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeEffect());

        // Color flash effect
        if (colorFlashCoroutine != null)
            StopCoroutine(colorFlashCoroutine);
        colorFlashCoroutine = StartCoroutine(ColorFlashEffect());
    }

    // Shake the enemy to indicate damage
    private IEnumerator ShakeEffect()
    {
        originalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = originalPosition.x + Random.Range(-shakeIntensity, shakeIntensity);
            float y = originalPosition.y + Random.Range(-shakeIntensity, shakeIntensity);
            transform.position = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        shakeCoroutine = null;
    }

    // Flash the sprite color to indicate damage
    private IEnumerator ColorFlashEffect()
    {
        if (!spriteRenderer) yield break;

        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(colorFlashDuration);
        spriteRenderer.color = originalColor;
        colorFlashCoroutine = null;
    }

    // Function to handle enemy death
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[EnemyHealth] Enemy died!");

        // Gain XP for the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var components = player.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                var typeName = component.GetType().Name;
                if (typeName == "SimplePlayerXP")
                {
                    var gainXPMethod = component.GetType().GetMethod("GainXP");
                    if (gainXPMethod != null)
                    {
                        gainXPMethod.Invoke(component, new object[] { xpReward });
                        Debug.Log($"[EnemyHealth] Gave {xpReward} XP to player");
                        break;
                    }
                }
            }
        }


        // Stop movement components
        var patrol = GetComponent<ModularEnemy>();
        if (patrol) patrol.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;

        // Destroy the enemy object after a delay
        if (destroyOnDeath)
        {
            StartCoroutine(DestroyAfterDelay());
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }

    // Function to check if the enemy is dead
    public bool IsDead()
    {
        return isDead;
    }

    // Function to get the current health percentage
    public float GetHealthPercentage()
    {
        return (float)CurrentHealth / maxHealth;
    }
}