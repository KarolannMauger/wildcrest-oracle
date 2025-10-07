using UnityEngine;

public class CowNPC : MonoBehaviour
{
    [Header("References")]
    public SimpleDialogSystem dialogSystem;
    
    void Start()
    {
        // Find dialog system if not assigned
        if (dialogSystem == null)
        {
            dialogSystem = GetComponent<SimpleDialogSystem>();
        }
        
        if (dialogSystem == null)
        {
            enabled = false;
        }
    }
    
    // Trigger dialog manually if needed
    public void TriggerDialog()
    {
        if (dialogSystem != null)
        {
            dialogSystem.ResetDialog();
        }
    }
}