using UnityEngine;
 
public class FadeOut2D : MonoBehaviour
{
    public float fadeDuration = 1f;     
    public bool destroyAfter = true;    
 
    private SpriteRenderer sr;          
    private bool fading = false;        
    private float t = 0f;               
 
    // Initialize components
    void Start()
    {
        
        sr = GetComponent<SpriteRenderer>();
    }
 
    // Handle the fade out effect
    void Update()
    {
        if (!fading) return;
        //
        t += Time.unscaledDeltaTime;
        float alpha = Mathf.Clamp01(1f - (t / fadeDuration));
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
        if (t >= fadeDuration)
        {
            if (destroyAfter) Destroy(gameObject);
            else gameObject.SetActive(false);
            
            Time.timeScale = 0f;
        }
    }
 
     // Trigger fade out on collision with "Oracle"
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Oracle"))
        {
            fading = true;
            MoleMovement move = GetComponent<MoleMovement>();
            if (move != null)
            {
                move.enabled = false;
            }
            // Disable all colliders
        }
    }
}