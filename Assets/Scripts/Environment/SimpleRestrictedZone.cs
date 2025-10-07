using UnityEngine;

public class SimpleRestrictedZone : MonoBehaviour
{
    [Header("Settings")]
    public int requiredLevel = 4; 
    public GameObject blockingWall; 
    public float pushForce = 1000f;
    
    [Header("Oracle Dialog")]
    public Transform oracleNPC; // Reference to Oracle NPC
    public string oracleMessage = "You don't have the necessary XP to come see me yet...";
    public float dialogDisplayTime = 3f;
    
    private SimplePlayerXP playerXP;
    private bool wasAccessible = false;
    private Collider2D zoneCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        zoneCollider = GetComponent<Collider2D>();
        if (zoneCollider != null)
        {
            zoneCollider.isTrigger = true; 
        }

        // Find SimplePlayerXP
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerXP = player.GetComponent<SimplePlayerXP>();
        }
        
        if (playerXP == null)
        {
            enabled = false;
            return;
        }

        // Create an invisible wall if not provided
        if (blockingWall == null)
        {
            CreateInvisibleWall();
        }
        
        UpdateZoneAccess();
    }
    
    // Create an invisible wall to block access
    void CreateInvisibleWall()
    {
        blockingWall = new GameObject("InvisibleWall");
        blockingWall.transform.SetParent(transform);
        blockingWall.transform.localPosition = Vector3.zero;

        // Add a solid collider (not trigger)
        var wallCollider = blockingWall.AddComponent<BoxCollider2D>();
        wallCollider.isTrigger = false;

        // Same size as the zone
        if (zoneCollider is BoxCollider2D boxCol)
        {
            wallCollider.size = boxCol.size;
            wallCollider.offset = boxCol.offset;
        }
        else if (zoneCollider is CircleCollider2D circleCol)
        {
            var newBoxCol = blockingWall.AddComponent<BoxCollider2D>();
            float diameter = circleCol.radius * 2;
            newBoxCol.size = new Vector2(diameter, diameter);
            Destroy(wallCollider);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateZoneAccess();
    }
    
    // Check if the player can access the zone and update wall visibility
    void UpdateZoneAccess()
    {
        if (playerXP == null) return;
        
        bool canAccess = playerXP.currentLevel >= requiredLevel;
        
        if (canAccess != wasAccessible)
        {
            wasAccessible = canAccess;
            
            if (GetComponent<SpriteRenderer>())
            {
                GetComponent<SpriteRenderer>().color = canAccess ? Color.green : Color.red;
            }
            
            // Enable/disable blocking wall
            if (blockingWall != null)
            {
                blockingWall.SetActive(!canAccess);
            }
        }
    }
    
    // Block the player from entering if they don't meet the level requirement
    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (playerXP.currentLevel < requiredLevel)
        {
            // Continuously push the player if they try to stay
            Vector2 pushDirection = (other.transform.position - transform.position).normalized;
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.AddForce(pushDirection * pushForce * Time.deltaTime);
            }
        }
    }
    
    // Notify player when they enter the zone
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (playerXP.currentLevel < requiredLevel)
        {
            ShowOracleDialog();
        }
    }
    
    // Show Oracle dialog
    void ShowOracleDialog()
    {
        if (oracleNPC == null) return;
        
        // Use SendMessage to trigger dialog
        oracleNPC.SendMessage("TriggerOracleDialog", new OracleDialogData { message = oracleMessage, displayTime = dialogDisplayTime }, SendMessageOptions.DontRequireReceiver);
    }
    
    // Structure to pass dialog data
    [System.Serializable]
    public class OracleDialogData
    {
        public string message;
        public float displayTime;
    }
}