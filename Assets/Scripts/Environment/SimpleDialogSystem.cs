using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SimpleDialogSystem : MonoBehaviour
{
    [Header("Dialog Settings")]
    public string[] dialogLines = {
        "Bienvenu Luma, j'ai attendu que tu cherche ton frère,",
        "seul l'Oracle peut tout te dire.",
        "Cependant pour rencontrer l'Oracle tu dois chasser",
        "les chats sauvages et les cochons verts.",
        "Aide-toi des pêches dans la forêt pour survivre.",
        "L'Oracle se trouve tout en haut à droite du village",
        "sur une petite colline."
    };
    
    [Header("UI References")]
    public GameObject dialogBox; // Le sprite de la box dialog
    public TextMeshProUGUI dialogText; 
    public float textSpeed = 0.05f; // Vitesse d'apparition du texte
    public float autoAdvanceDelay = 3f; // Délai avant passage automatique
    
    [Header("Detection")]
    public float detectionRadius = 2f;
    public LayerMask playerLayer = -1;
    
    [Header("Controls")]
    public KeyCode advanceKey = KeyCode.F;
    public bool useAutoAdvance = true; // true = auto, false = manuel avec F
    
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
        // Trouver le joueur
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        // Cacher la dialog box au début
        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }
        
        // Vérifier les références
        if (dialogText == null)
        {
            Debug.LogError("[SimpleDialogSystem] Dialog Text not assigned!");
        }
        
        if (dialogBox == null)
        {
            Debug.LogError("[SimpleDialogSystem] Dialog Box not assigned!");
        }
        
        Debug.Log($"[SimpleDialogSystem] Initialized - Auto advance: {useAutoAdvance}");
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
            // Joueur entre dans la zone
            playerInRange = true;
            StartDialog();
        }
        else if (!playerNearby && playerInRange)
        {
            // Joueur sort de la zone
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
        
        Debug.Log("[SimpleDialogSystem] Dialog started");
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
        
        // Commencer l'animation de texte
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
        
        // Texte terminé, démarrer auto-advance si activé
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
        Debug.Log("[SimpleDialogSystem] Dialog completed!");
    }
    
    void EndDialog()
    {
        dialogActive = false;
        
        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }
        
        // Arrêter toutes les coroutines
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
        }
        
        Debug.Log("[SimpleDialogSystem] Dialog ended");
    }
    
    // Méthode pour réinitialiser le dialogue (optionnel)
    public void ResetDialog()
    {
        dialogCompleted = false;
        currentLineIndex = 0;
        EndDialog();
        Debug.Log("[SimpleDialogSystem] Dialog reset");
    }
    
    // Dessiner la zone de détection dans l'éditeur
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // Dessiner une ligne vers le joueur si trouvé
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            Gizmos.color = distance <= detectionRadius ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}