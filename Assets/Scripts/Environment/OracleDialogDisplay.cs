using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class OracleDialogDisplay : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogBox; // Your existing DialogBox
    public TextMeshProUGUI dialogText; // Your existing Text
    
    [Header("Dialog Settings")]
    public Vector3 dialogOffset = new Vector3(0, 2, 0); // Above the Oracle
    
    private bool isShowing = false;
    private Coroutine hideCoroutine;
    
    void Start()
    {
        // Check references
        if (dialogBox == null || dialogText == null)
        {
            enabled = false;
            return;
        }
        
        // Hide dialog box at start
        dialogBox.SetActive(false);
        
        // Position dialog box above Oracle
        dialogBox.transform.position = transform.position + dialogOffset;
    }
    

    
    public void ShowMessage(string message, float displayTime)
    {
        if (isShowing) return;
        
        isShowing = true;
        
        if (dialogText != null)
        {
            dialogText.text = message;
        }
        
        if (dialogBox != null)
        {
            dialogBox.SetActive(true);
        }
        
        // Schedule automatic close
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(HideAfterDelay(displayTime));
    }
    
    // Method to receive messages via SendMessage
    public void TriggerOracleDialog(SimpleRestrictedZone.OracleDialogData data)
    {
        ShowMessage(data.message, data.displayTime);
    }
    
    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideMessage();
    }
    
    public void HideMessage()
    {
        isShowing = false;
        
        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }
    }
    
    // Method to change dialog position
    public void SetDialogOffset(Vector3 offset)
    {
        dialogOffset = offset;
        if (dialogBox != null)
        {
            dialogBox.transform.position = transform.position + dialogOffset;
        }
    }
}