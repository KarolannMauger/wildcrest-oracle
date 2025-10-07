using UnityEngine;

public class SimpleRestrictedZone : MonoBehaviour
{
    [Header("Settings")]
    public int requiredLevel = 4; 
    public GameObject blockingWall; 
    public float pushForce = 1000f;
    
    [Header("Oracle Dialog")]
    public Transform oracleNPC; // Référence vers l'Oracle NPC
    public string oracleMessage = "Vous n'avez pas encore l'XP nécessaire pour venir me voir...";
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
            Debug.LogError("[SimpleRestrictedZone] SimplePlayerXP not found!");
            return;
        }

        // Create an invisible wall if not provided
        if (blockingWall == null)
        {
            CreateInvisibleWall();
        }
        
        Debug.Log($"[SimpleRestrictedZone] Zone requires level {requiredLevel}");
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
        
        Debug.Log("[SimpleRestrictedZone] Created invisible wall");
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
            
            if (canAccess)
            {
                Debug.Log("[SimpleRestrictedZone] ACCESS GRANTED! Zone is now open.");
                if (GetComponent<SpriteRenderer>())
                    GetComponent<SpriteRenderer>().color = Color.green;
            }
            else
            {
                Debug.Log("[SimpleRestrictedZone] Access denied - need level " + requiredLevel);
                if (GetComponent<SpriteRenderer>())
                    GetComponent<SpriteRenderer>().color = Color.red;
            }
            
            // Activer/désactiver le mur bloquant
            if (blockingWall != null)
            {
                blockingWall.SetActive(!canAccess);
                Debug.Log($"[SimpleRestrictedZone] Wall {(canAccess ? "disabled" : "enabled")}");
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
    
    // Notify the player when they enter the zone
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (playerXP.currentLevel < requiredLevel)
        {
            Debug.Log($"[SimpleRestrictedZone] Access denied! You are level {playerXP.currentLevel}, need level {requiredLevel}");
            ShowOracleDialog();
        }
        else
        {
            Debug.Log("[SimpleRestrictedZone] Welcome to the restricted area!");
        }
    }
    
    // Afficher le dialogue de l'Oracle
    void ShowOracleDialog()
    {
        if (oracleNPC == null)
        {
            Debug.LogWarning("[SimpleRestrictedZone] Oracle NPC not assigned!");
            return;
        }
        
        // Utiliser SendMessage pour déclencher le dialogue
        oracleNPC.SendMessage("TriggerOracleDialog", new OracleDialogData { message = oracleMessage, displayTime = dialogDisplayTime }, SendMessageOptions.DontRequireReceiver);
        
        Debug.Log("[SimpleRestrictedZone] Oracle dialog triggered");
    }
    
    // Structure pour passer les données du dialogue
    [System.Serializable]
    public class OracleDialogData
    {
        public string message;
        public float displayTime;
    }
}