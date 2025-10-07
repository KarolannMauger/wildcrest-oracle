using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 10;
    public int CurrentHealth { get; private set; }

    [Header("Death / Reload")]
    public float reloadDelay = 3f;
    public string sceneToLoad = "GameOver";
    private string deathTriggerName = "dead";
    private string respawnTriggerName = "respawn";

    [Header("Damage Effects")]
    public float shakeDuration = 0.3f;
    public float shakeIntensity = 0.1f;
    public Color damageColor = Color.red;
    public float colorFlashDuration = 0.2f;

    [Header("Audio")]
    public AudioClip takeDamageSfx;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isDead;
    private Vector3 originalPosition;
    private Color originalColor;
    private Coroutine shakeCoroutine;
    private Coroutine colorFlashCoroutine;
    
    [Header("Respawn")]
    public bool useRespawn = true;
    public int maxRespawns = 2;
    private Vector3 lastSafePosition;
    private SimplePlayerXP playerXP;
    private int respawnCount = 0;

    // Initialize components and variables
    void Awake()
    {
        CurrentHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        playerXP = GetComponent<SimplePlayerXP>();
        
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        originalPosition = transform.position;
        lastSafePosition = transform.position; // Initial position as safe position
        if (spriteRenderer)
            originalColor = spriteRenderer.color;
    }
    
    void Update()
    {
        // Save position regularly when player is not in danger
        if (!isDead && CurrentHealth > 0)
        {
            lastSafePosition = transform.position;
        }
    }

    // Apply damage to the player
    public void TakeDamage(int dmg)
    {
        if (isDead || dmg <= 0) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - dmg);

        // Play damage effects
        PlayDamageEffects();

        if (CurrentHealth <= 0) Die();
    }

    // Heal player if possible and return true if health was gained
    public bool TryHeal(int amount)
    {
        if (amount <= 0) return false;

        if (CurrentHealth >= maxHealth)
            return false;

        int old = CurrentHealth;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        int gained = CurrentHealth - old;

        return gained > 0;
    }

    // Handle player death
    void Die()
    {
        if (isDead) return;

        isDead = true;

        if (animator && !string.IsNullOrEmpty(deathTriggerName))
            animator.SetTrigger(deathTriggerName);

        if (useRespawn)
        {
            // Check if player can still respawn
            if (respawnCount < maxRespawns)
            {
                // Respawn after delay
                StartCoroutine(RespawnAfterDelay());
            }
            else
            {
                // No more respawns available → Game Over
                if (reloadDelay <= 0f) SceneManager.LoadScene(sceneToLoad);
                else StartCoroutine(CoDelay(sceneToLoad, reloadDelay));
            }
        }
        else
        {
            // Original system - reload scene
            if (reloadDelay <= 0f) SceneManager.LoadScene(sceneToLoad);
            else StartCoroutine(CoDelay(sceneToLoad, reloadDelay));
        }
    }

    // Respawn after delay
    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSecondsRealtime(reloadDelay);
        Respawn();
    }

    // Respawn function
    private void Respawn()
    {
        respawnCount++;
        
        // 1. Restore health to maximum
        CurrentHealth = maxHealth;
        
        // 2. Reposition player to last safe position
        transform.position = lastSafePosition;
        
        // 3. XP remains intact - no reset
        
        // 4. Reset death animation
        if (animator != null)
        {
            // Reset death trigger and activate respawn
            animator.ResetTrigger(deathTriggerName);
            animator.SetTrigger(respawnTriggerName);
            animator.SetFloat("speed", 0f);
        }
        
        // 5. Set isDead to false
        isDead = false;
    }

    // Play visual and audio effects when player takes damage
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

        // Flash effect
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

    // Delay before reloading scene
    IEnumerator CoDelay(string name, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SceneManager.LoadScene(name);
    }
    
}
