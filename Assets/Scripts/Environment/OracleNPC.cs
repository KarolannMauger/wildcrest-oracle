using UnityEngine;

public class OracleNPC : MonoBehaviour
{
    [Header("Victory Dialog")]
    public string victoryMessage = "Congratulations Luma, your brother is hiding in the wildcrest ruins, come with me I will guide you to the ruins.";
    public float dialogDuration = 5f;
    public int requiredLevel = 4;
    
    private SimplePlayerXP playerXP;
    private OracleDialogDisplay dialogDisplay;
    private bool victoryTriggered = false;
    
    void Start()
    {
        // Find required components
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerXP = player.GetComponent<SimplePlayerXP>();
        }
        
        dialogDisplay = GetComponent<OracleDialogDisplay>();
        
        if (playerXP == null || dialogDisplay == null)
        {
            enabled = false;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || victoryTriggered) return;
        
        if (playerXP == null) return;
        
        if (playerXP.currentLevel >= requiredLevel)
        {
            ShowVictoryMessage();
        }
    }
    
    void ShowVictoryMessage()
    {
        if (victoryTriggered) return;
        
        victoryTriggered = true;
        
        // Show victory dialog
        if (dialogDisplay != null)
        {
            dialogDisplay.ShowMessage(victoryMessage, dialogDuration);
        }
    }
    
    // Public method to check if victory was triggered
    public bool IsVictoryTriggered()
    {
        return victoryTriggered;
    }
    
}