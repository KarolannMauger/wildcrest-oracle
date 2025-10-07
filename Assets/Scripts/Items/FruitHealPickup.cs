using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FruitHealPickup : MonoBehaviour
{
    public int healAmount = 1;
    public bool destroyOnPickup = true;
    public AudioClip pickupSfx;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var hp = other.GetComponent<PlayerHealth>();
        if (!hp) return;

        // Try to heal; only consume fruit if HP was actually gained
        bool healed = hp.TryHeal(healAmount);

        if (!healed) return;

        // Play pickup sound and destroy
        if (pickupSfx) AudioSource.PlayClipAtPoint(pickupSfx, transform.position);
        
        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }
}
