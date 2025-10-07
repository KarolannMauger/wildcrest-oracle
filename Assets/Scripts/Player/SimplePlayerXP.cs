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
    
    void Start()
    {
        Debug.Log($"[SimplePlayerXP] Initialized - Level {currentLevel}, Max Level {maxLevel}");
    }
    
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
        CheckLevelUp();
    }
    
    void CheckLevelUp()
    {
        if (currentLevel < maxLevel && currentXP >= GetXPNeededForNextLevel())
        {
            currentLevel++;
            Debug.Log($"[SimplePlayerXP] LEVEL UP! Now level {currentLevel}");
            
            if (IsMaxLevel)
            {
                Debug.Log("[SimplePlayerXP] MAX LEVEL REACHED!");
            }
        }
    }
    
    public float GetXPNeededForNextLevel()
    {
        if (IsMaxLevel) return 0f;
        return currentLevel + 1;
    }
    
    public float GetLevelProgress()
    {
        if (IsMaxLevel) return 1f;
        
        float xpForCurrentLevel = currentLevel;
        float xpForNextLevel = GetXPNeededForNextLevel();
        float progressInLevel = currentXP - xpForCurrentLevel;
        float xpNeededForLevelUp = xpForNextLevel - xpForCurrentLevel;
        
        if (xpNeededForLevelUp <= 0) return 1f;
        
        return Mathf.Clamp01(progressInLevel / xpNeededForLevelUp);
    }
    
    public float GetXPToNextLevel()
    {
        if (IsMaxLevel) return 0f;
        return GetXPNeededForNextLevel() - currentXP;
    }
    
    public float GetPercentageToNextLevel()
    {
        if (IsMaxLevel) return 100f;
        
        float needed = GetXPNeededForNextLevel();
        if (needed <= 0) return 100f;
        
        return (currentXP / needed) * 100f;
    }
    
    public string GetLevelInfo()
    {
        if (IsMaxLevel)
        {
            return $"MAX (Level {currentLevel})";
        }
        
        return $"Level {currentLevel} ({GetPercentageToNextLevel():F1}%)";
    }
    
    [ContextMenu("Add 1 XP (0.5)")]
    void DebugAddXP()
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
    
    // Methods for UI Display
    public float GetXPPercentage()
    {
        return GetLevelProgress();
    }
    
    public string GetXPDisplayText()
    {
        return GetLevelInfo();
    }
    
    public string GetXPProgressText()
    {
        if (IsMaxLevel)
        {
            return "MAX LEVEL";
        }
        
        return $"{currentXP:F1}/{GetXPNeededForNextLevel():F1} XP";
    }
}