using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class PlayerHealth : MonoBehaviour
{
    [Header("PV")]
    public int maxHealth = 10;
    public int CurrentHealth { get; private set; }

    [Header("Dead / Reload")]
    public float reloadDelay = 3f;
    public string sceneToLoad = "GameOver";
    private string deathTriggerName = "dead";

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

    void Awake()
    {
        CurrentHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        originalPosition = transform.position;
        if (spriteRenderer)
            originalColor = spriteRenderer.color;

        Debug.Log($"[PlayerHealth] Init HP = {CurrentHealth}/{maxHealth}");
    }

    public void TakeDamage(int dmg)
    {
        if (isDead || dmg <= 0) return;
        
        CurrentHealth = Mathf.Max(0, CurrentHealth - dmg);
        Debug.Log($"[PlayerHealth] -{dmg} HP → {CurrentHealth}/{maxHealth}");
        
        // Effets de dégâts
        PlayDamageEffects();
        
        if (CurrentHealth <= 0) Die();
    }

    // Heals if possible and returns true if health points were gained.
    public bool TryHeal(int amount)
    {
        if (amount <= 0) return false;

        if (CurrentHealth >= maxHealth)
        {
            Debug.Log("[PlayerHealth] Health full → no healing.");
            return false;
        }

        int old = CurrentHealth;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        int gained = CurrentHealth - old;

        Debug.Log($"[PlayerHealth] +{gained} HP → {CurrentHealth}/{maxHealth}");
        return gained > 0;
    }

    void Die()
    {
        if (isDead) return;

        Debug.Log("Player is dead!");

        if (animator && !string.IsNullOrEmpty(deathTriggerName))
            animator.SetTrigger(deathTriggerName);

        if (reloadDelay <= 0f) SceneManager.LoadScene(sceneToLoad);
        else StartCoroutine(CoDelay(sceneToLoad, reloadDelay));
    }
 
    private void PlayDamageEffects()
    {
        // Son de dégâts
        if (takeDamageSfx && audioSource)
        {
            audioSource.PlayOneShot(takeDamageSfx);
        }

        // Shake effect
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeEffect());

        // Flash de couleur
        if (colorFlashCoroutine != null)
            StopCoroutine(colorFlashCoroutine);
        colorFlashCoroutine = StartCoroutine(ColorFlashEffect());
    }

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

    private IEnumerator ColorFlashEffect()
    {
        if (!spriteRenderer) yield break;

        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(colorFlashDuration);
        spriteRenderer.color = originalColor;
        colorFlashCoroutine = null;
    }

    IEnumerator CoDelay(string name, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SceneManager.LoadScene(name);
    }
    
}
