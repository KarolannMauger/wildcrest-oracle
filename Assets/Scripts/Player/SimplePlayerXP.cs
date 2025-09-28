using UnityEngine;

public class SimplePlayerXP : MonoBehaviour
{
    [Header("XP Settings")]
    public int maxLevel = 5; 
    public float xpPerEnemy = 0.5f;
    
    [Header("Current Stats - Read Only")]
    public int currentLevel = 0;
    public float currentXP = 0f; 
    
    public bool IsMaxLevel => currentLevel >= maxLevel;
    
    // Initialize references
    void Start()
    {
        Debug.Log($"[SimplePlayerXP] Initialized - Level {currentLevel}, Max Level {maxLevel}");
    }
    
    // Gain XP from defeated enemies
    public void GainXP(int enemiesKilled)
    {
        if (IsMaxLevel)
        {
            Debug.Log("[SimplePlayerXP] Already at max level!");
            return;
        }

        float xpGained = enemiesKilled * xpPerEnemy;
        currentXP += xpGained;

        Debug.Log($"[SimplePlayerXP] Gained {xpGained} XP. Current XP: {currentXP}");

        // Check for level ups
        while (currentXP >= 1f && !IsMaxLevel)
        {
            currentXP -= 1f; // Reset XP bar
            currentLevel++;
            Debug.Log($"[SimplePlayerXP] LEVEL UP! Now level {currentLevel}");

            if (IsMaxLevel)
            {
                currentXP = 0f; 
                Debug.Log("[SimplePlayerXP] MAX LEVEL REACHED!");
                break;
            }
        }
    }
    
    // Get current XP percentage (0.0 to 1.0)
    public float GetXPPercentage()
    {
        if (IsMaxLevel) return 1f;
        return currentXP;
    }
    
    // Get display text for current level
    public string GetXPDisplayText()
    {
        if (IsMaxLevel)
        {
            return $"Level {currentLevel} - MAX";
        }
        else
        {
            int nextLevel = currentLevel + 1;
            return $"Level {currentLevel} → {nextLevel}";
        }
    }
    
    // Get progress text for current XP (e.g., "1/2 XP")
    public string GetXPProgressText()
    {
        if (IsMaxLevel)
        {
            return "MAX LEVEL";
        }
        else
        {
            int currentXPDisplay = Mathf.FloorToInt(currentXP * 2);
            return $"{currentXPDisplay}/2 XP";
        }
    }
    
    // Test methods
    [ContextMenu("Add 1 XP (0.5)")]
    public void TestAddXP()
    {
        GainXP(1);
    }
    
    [ContextMenu("Reset Level")]
    public void ResetLevel()
    {
        currentLevel = 0;
        currentXP = 0f;
        Debug.Log("[SimplePlayerXP] Reset to level 0");
    }
}