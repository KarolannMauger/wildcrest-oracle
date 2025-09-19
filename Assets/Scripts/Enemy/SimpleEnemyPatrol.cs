using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class SimpleEnemyPatrol : MonoBehaviour
{
    public Transform leftPoint, rightPoint;
    public float speed = 2f;
    public int touchDamage = 1;

    [Header("Audio")]
    public AudioClip hitSfx;
    private AudioSource audioSource;

    Rigidbody2D rb;
    SpriteRenderer sr;
    bool toRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        Debug.Log($"[Enemy] Init patrol entre {leftPoint?.name} et {rightPoint?.name}");
    }

    void FixedUpdate()
    {
        float dir = toRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * speed, 0f);
        sr.flipX = !toRight;

        if (toRight && transform.position.x >= rightPoint.position.x)
        {
            toRight = false;
        }
        else if (!toRight && transform.position.x <= leftPoint.position.x)
        {
            toRight = true;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")){return;}

        var hp = col.collider.GetComponent<PlayerHealth>();
        if (hp)
        {
            Debug.Log($"[Enemy] Inflicts {touchDamage} dmg to Player");
            hp.TakeDamage(touchDamage);
            if (hitSfx != null)
            {
                audioSource.PlayOneShot(hitSfx);
            }
        }
    }
}
