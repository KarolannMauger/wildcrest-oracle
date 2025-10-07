using UnityEngine;

public class OracleNPC : MonoBehaviour
{
    [Header("Victory Dialog")]
    public string victoryMessage = "Félicitation à toi Luma, ton frère se cache dans les ruines du wildcrest, vient avec moi je vais te guider jusqu'au ruine.";
    public float dialogDuration = 5f;
    public int requiredLevel = 4;
    
    private SimplePlayerXP playerXP;
    private OracleDialogDisplay dialogDisplay;
    private bool victoryTriggered = false;
    
    void Start()
    {
        // Trouver les composants nécessaires
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerXP = player.GetComponent<SimplePlayerXP>();
        }
        
        dialogDisplay = GetComponent<OracleDialogDisplay>();
        
        if (playerXP == null)
        {
            Debug.LogError("[OracleNPC] SimplePlayerXP not found on player!");
        }
        
        if (dialogDisplay == null)
        {
            Debug.LogError("[OracleNPC] OracleDialogDisplay not found on Oracle!");
        }
        
        Debug.Log($"[OracleNPC] Oracle ready - requires level {requiredLevel}");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || victoryTriggered) return;
        
        if (playerXP == null) return;
        
        if (playerXP.currentLevel >= requiredLevel)
        {
            Debug.Log($"[OracleNPC] Player reached level {playerXP.currentLevel} - showing victory message!");
            ShowVictoryMessage();
        }
        else
        {
            Debug.Log($"[OracleNPC] Player level {playerXP.currentLevel} - need level {requiredLevel}");
        }
    }
    
    void ShowVictoryMessage()
    {
        if (victoryTriggered) return;
        
        victoryTriggered = true;
        
        // Afficher le dialogue de victoire
        if (dialogDisplay != null)
        {
            dialogDisplay.ShowMessage(victoryMessage, dialogDuration);
        }
        
        Debug.Log("[OracleNPC] Victory message displayed - your existing fadeout script will handle the rest!");
    }
    
    // Méthode publique pour vérifier si la victoire a été déclenchée
    public bool IsVictoryTriggered()
    {
        return victoryTriggered;
    }
    
}