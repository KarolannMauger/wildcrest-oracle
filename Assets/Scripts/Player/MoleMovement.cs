using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(PlayerInput))]
public class MoleMovement : MonoBehaviour
{
    [Header("Mouvement")]
    public float speed = 5f;
    public bool normalize = true;

    [Header("Audio")]
    public AudioClip moveClip;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDir = Vector2.down;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;


    // Initialize components
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = moveClip;
    }

    // Handle movement input
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        if (normalize && moveInput.sqrMagnitude > 1f)
            moveInput = moveInput.normalized;
    }

    // Handle physics and movement
    void FixedUpdate()
    {
        // Move the player
        if (moveInput.sqrMagnitude > 0f)
        {
            lastMoveDir = moveInput;
            Vector2 target = rb.position + moveInput * speed * Time.fixedDeltaTime;
            rb.MovePosition(target);
            if (moveClip != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        // Determine direction for animation
        int dir = 0;
        float absX = Mathf.Abs(lastMoveDir.x);
        float absY = Mathf.Abs(lastMoveDir.y);
        if (absX > 0.1f && absY > 0.1f)
        {
            if (absX > absY)
                dir = 2;
            else
                dir = lastMoveDir.y > 0 ? 0 : 3;
        }
        else if (absX > 0.1f)
        {
            dir = 2;
        }
        else if (absY > 0.1f)
        {
            dir = lastMoveDir.y > 0 ? 0 : 3;
        }

        //Debug.Log($"moveInput: {moveInput}, lastMoveDir: {lastMoveDir}, lastMoveDir.y: {lastMoveDir.y}, dir: {dir}");

        //Animator Speed and Dir
        animator.SetFloat("speed", moveInput.sqrMagnitude);
        animator.SetInteger("dir", dir);

        //FlipX
        if (spriteRenderer != null)
        {
            if (dir == 2)
                spriteRenderer.flipX = lastMoveDir.x < 0;
            else
                spriteRenderer.flipX = false;
        }
    }
}