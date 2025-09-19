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

    private Animator animator;
    private bool isDead;

    void Awake()
    {
        CurrentHealth = maxHealth;
        animator = GetComponent<Animator>();
        Debug.Log($"[PlayerHealth] Init HP = {CurrentHealth}/{maxHealth}");
    }

    public void TakeDamage(int dmg)
    {
        if (dmg <= 0) return;
        CurrentHealth = Mathf.Max(0, CurrentHealth - dmg);
        Debug.Log($"[PlayerHealth] -{dmg} HP → {CurrentHealth}/{maxHealth}");
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
 
       IEnumerator CoDelay(string name, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SceneManager.LoadScene(name);
    }
    
}
