using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public int CurrentHealth { get; private set; }

    [Header("Death")]
    public bool destroyOnDeath = true;
    public float deathDelay = 0.5f;

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
    }

    // Apply damage to the enemy
    public void TakeDamage(int dmg)
    {
        if (isDead || dmg <= 0) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - dmg);

        // Play damage effects
        PlayDamageEffects();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    // Play visual and audio effects when enemy takes damage
    private void PlayDamageEffects()
    {
        // Play damage sound effect
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

    // Shake effect to indicate damage
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

    // Flash sprite color to indicate damage
    private IEnumerator ColorFlashEffect()
    {
        if (!spriteRenderer) yield break;

        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(colorFlashDuration);
        spriteRenderer.color = originalColor;
        colorFlashCoroutine = null;
    }

    // Handle enemy death
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Play death sound effect
        if (deathSfx && audioSource)
        {
            audioSource.PlayOneShot(deathSfx);
        }

        // Give XP to player
        GiveXPToPlayer();

        // Stop movement components
        var patrol = GetComponent<ModularEnemy>();
        if (patrol) patrol.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;

        // Destroy the enemy object after delay
        if (destroyOnDeath)
        {
            StartCoroutine(DestroyAfterDelay());
        }
    }

    // Give XP reward to player
    private void GiveXPToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SimplePlayerXP playerXP = player.GetComponent<SimplePlayerXP>();
            if (playerXP != null)
            {
                playerXP.GainXP(xpReward);
            }
        }
    }

    // Destroy enemy after delay
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }

    // Check if enemy is dead
    public bool IsDead()
    {
        return isDead;
    }

    // Get current health percentage
    public float GetHealthPercentage()
    {
        return (float)CurrentHealth / maxHealth;
    }
}