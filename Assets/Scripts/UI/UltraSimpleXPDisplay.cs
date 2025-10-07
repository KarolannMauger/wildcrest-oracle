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
        // Find SimplePlayerXP
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerXP = player.GetComponent<SimplePlayerXP>();
        }
        
        if (playerXP == null)
        {
            enabled = false;
            return;
        }
        
        // Configure slider
        if (xpSlider != null)
        {
            xpSlider.minValue = 0f;
            xpSlider.maxValue = 1f;
        }
    }

    // Update the XP display
    void Update()
    {
        if (playerXP == null) return;

        // Update slider (XP bar)
        if (xpSlider != null)
        {
            xpSlider.value = playerXP.GetXPPercentage();
        }

        // Update level text
        if (levelText != null)
        {
            levelText.text = playerXP.GetXPDisplayText();
        }

        // Update XP progress text
        if (xpProgressText != null)
        {
            xpProgressText.text = playerXP.GetXPProgressText();
        }
    }
}