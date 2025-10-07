using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SimpleDialogSystem : MonoBehaviour
{
    [Header("Dialog Settings")]
    public string[] dialogLines = {
        "Welcome Luma, I have been waiting for you to search for your brother,",
        "only the Oracle can tell you everything.",
        "However to meet the Oracle you must hunt",
        "wild cats and green pigs.",
        "Use the peaches in the forest to survive.",
        "The Oracle is located at the top right of the village",
        "on a small hill."
    };
    
    [Header("UI References")]
    public GameObject dialogBox; // Dialog box sprite
    public TextMeshProUGUI dialogText; 
    public float textSpeed = 0.05f; // Text appearance speed
    public float autoAdvanceDelay = 3f; // Delay before auto advance
    
    [Header("Detection")]
    public float detectionRadius = 2f;
    public LayerMask playerLayer = -1;
    
    [Header("Controls")]
    public KeyCode advanceKey = KeyCode.F;
    public bool useAutoAdvance = true; // true = auto, false = manual with F
    
    // Private variables
    private Transform player;
    private bool playerInRange = false;
    private bool dialogActive = false;
    private bool dialogCompleted = false;
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;
    private Coroutine autoAdvanceCoroutine;
    
    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        // Hide dialog box at start
        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }
        
        // Check references
        if (dialogText == null || dialogBox == null)
        {
            enabled = false;
        }
    }
    
    void Update()
    {
        CheckPlayerDistance();
        HandleInput();
    }
    
    void CheckPlayerDistance()
    {
        if (player == null || dialogCompleted) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        bool playerNearby = distance <= detectionRadius;
        
        if (playerNearby && !playerInRange)
        {
            // Player enters zone
            playerInRange = true;
            StartDialog();
        }
        else if (!playerNearby && playerInRange)
        {
            // Player exits zone
            playerInRange = false;
            EndDialog();
        }
    }
    
    void HandleInput()
    {
        if (!dialogActive || useAutoAdvance) return;
        
        if (Input.GetKeyDown(advanceKey))
        {
            AdvanceDialog();
        }
    }
    
    void StartDialog()
    {
        if (dialogCompleted) return;
        
        dialogActive = true;
        currentLineIndex = 0;
        
        if (dialogBox != null)
        {
            dialogBox.SetActive(true);
        }
        
        ShowCurrentLine();
    }
    
    void ShowCurrentLine()
    {
        if (currentLineIndex >= dialogLines.Length)
        {
            CompleteDialog();
            return;
        }
        
        string currentLine = dialogLines[currentLineIndex];
        
        // Arrêter les coroutines précédentes
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
        }
        
        // Start text animation
        typingCoroutine = StartCoroutine(TypeText(currentLine));
    }
    
    IEnumerator TypeText(string text)
    {
        if (dialogText == null) yield break;
        
        dialogText.text = "";
        
        foreach (char letter in text.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        
        // Text finished, start auto-advance if enabled
        if (useAutoAdvance)
        {
            autoAdvanceCoroutine = StartCoroutine(AutoAdvanceAfterDelay());
        }
    }
    
    IEnumerator AutoAdvanceAfterDelay()
    {
        yield return new WaitForSeconds(autoAdvanceDelay);
        AdvanceDialog();
    }
    
    void AdvanceDialog()
    {
        currentLineIndex++;
        ShowCurrentLine();
    }
    
    void CompleteDialog()
    {
        dialogCompleted = true;
        EndDialog();
    }
    
    void EndDialog()
    {
        dialogActive = false;
        
        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }
        
        // Stop all coroutines
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
        }
    }
    
    // Method to reset dialog (optional)
    public void ResetDialog()
    {
        dialogCompleted = false;
        currentLineIndex = 0;
        EndDialog();
    }
    
    // Draw detection zone in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // Draw line to player if found
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            Gizmos.color = distance <= detectionRadius ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}