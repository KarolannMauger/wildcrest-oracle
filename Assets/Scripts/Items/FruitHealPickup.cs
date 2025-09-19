using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FruitHealPickup : MonoBehaviour
{
    public int healAmount = 1;
    public bool destroyOnPickup = true;
    public AudioClip pickupSfx; // futur feature

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")){return;}

        var hp = other.GetComponent<PlayerHealth>();
        if (!hp)
        {
            Debug.Log("[FruitHealPickup] Player does not have PlayerHealth!");
            return;
        }

        // Try to heal; only consume the fruit if HP was actually gained
        bool healed = hp.TryHeal(healAmount);

        if (!healed)
        {
            Debug.Log("[FruitHealPickup] HP full → fruit NOT picked up.");
            return;
        }


        // futur feature
        if (pickupSfx) AudioSource.PlayClipAtPoint(pickupSfx, transform.position);
        if (destroyOnPickup)
        {
            Debug.Log("[FruitHealPickup] Pickup consumed and destroyed.");
            Destroy(gameObject);
        }
    }
}
