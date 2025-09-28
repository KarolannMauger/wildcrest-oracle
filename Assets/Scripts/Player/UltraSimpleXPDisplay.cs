using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltraSimpleXPDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Slider xpSlider;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpProgressText; // Nouveau: pour afficher "1/2 XP"
    
    private SimplePlayerXP playerXP;
    
    void Start()
    {
        // Trouver SimplePlayerXP
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerXP = player.GetComponent<SimplePlayerXP>();
        }
        
        if (playerXP == null)
        {
            Debug.LogError("[UltraSimpleXPDisplay] SimplePlayerXP not found!");
            enabled = false;
            return;
        }
        
        // Configurer le slider
        if (xpSlider != null)
        {
            xpSlider.minValue = 0f;
            xpSlider.maxValue = 1f;
        }
        
        Debug.Log("[UltraSimpleXPDisplay] Initialized successfully");
    }
    
    void Update()
    {
        if (playerXP == null) return;
        
        // Mettre à jour le slider (barre XP)
        if (xpSlider != null)
        {
            xpSlider.value = playerXP.GetXPPercentage();
        }
        
        // Mettre à jour le texte de niveau
        if (levelText != null)
        {
            levelText.text = playerXP.GetXPDisplayText();
        }
        
        // Mettre à jour le texte de progression XP
        if (xpProgressText != null)
        {
            xpProgressText.text = playerXP.GetXPProgressText();
        }
    }
}