using UnityEngine;

public class CowNPC : MonoBehaviour
{
    [Header("References")]
    public SimpleDialogSystem dialogSystem;
    
    void Start()
    {
        // Si pas de dialog system assigné, essayer de le trouver sur le même GameObject
        if (dialogSystem == null)
        {
            dialogSystem = GetComponent<SimpleDialogSystem>();
        }
        
        if (dialogSystem == null)
        {
            Debug.LogError("[CowNPC] SimpleDialogSystem not found!");
        }
        else
        {
            Debug.Log("[CowNPC] Cow NPC ready to talk!");
        }
    }
    
    // Méthode pour déclencher le dialogue manuellement si nécessaire
    public void TriggerDialog()
    {
        if (dialogSystem != null)
        {
            dialogSystem.ResetDialog();
        }
    }
}