using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OracleDialogDisplay : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogBox; // Votre DialogBox existant
    public Text dialogText; // Votre Text existant
    
    [Header("Dialog Settings")]
    public Vector3 dialogOffset = new Vector3(0, 2, 0); // Au-dessus de l'Oracle
    
    private bool isShowing = false;
    private Coroutine hideCoroutine;
    
    void Start()
    {
        // Vérifier les références
        if (dialogBox == null)
        {
            Debug.LogError("[OracleDialogDisplay] Dialog Box not assigned!");
        }
        
        if (dialogText == null)
        {
            Debug.LogError("[OracleDialogDisplay] Dialog Text not assigned!");
        }
        
        // Cacher la dialog box au début
        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }
        
        // Positionner la dialog box au-dessus de l'Oracle
        if (dialogBox != null)
        {
            dialogBox.transform.position = transform.position + dialogOffset;
        }
        
        Debug.Log("[OracleDialogDisplay] Using existing UI components");
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
        
        // Programmer la fermeture automatique
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(HideAfterDelay(displayTime));
        
        Debug.Log($"[OracleDialogDisplay] Showing message: {message}");
    }
    
    // Méthode pour recevoir les messages via SendMessage
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
        
        Debug.Log("[OracleDialogDisplay] Message hidden");
    }
    
    // Méthode pour changer la position du dialogue
    public void SetDialogOffset(Vector3 offset)
    {
        dialogOffset = offset;
        if (dialogBox != null)
        {
            dialogBox.transform.position = transform.position + dialogOffset;
        }
    }
}