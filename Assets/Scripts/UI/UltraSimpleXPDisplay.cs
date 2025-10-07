using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltraSimpleXPDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Slider xpSlider;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpProgressText;
    
    private SimplePlayerXP playerXP;
    
    // Initialize references
    void Start()
    {
        // Search SimplePlayerXP
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

    // Update the XP display
    void Update()
    {
        if (playerXP == null) return;

        // Update the slider (XP bar)
        if (xpSlider != null)
        {
            xpSlider.value = playerXP.GetXPPercentage();
        }

        // Update the level text
        if (levelText != null)
        {
            levelText.text = playerXP.GetXPDisplayText();
        }

        // Update the XP progress text
        if (xpProgressText != null)
        {
            xpProgressText.text = playerXP.GetXPProgressText();
        }
    }
}