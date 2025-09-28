using UnityEngine;

public class SimplePlayerXP : MonoBehaviour
{
    [Header("XP Settings")]
    public int maxLevel = 5; // 5 niveaux maximum
    public float xpPerEnemy = 0.5f; // Chaque ennemi donne 0.5 XP
    
    [Header("Current Stats - Read Only")]
    public int currentLevel = 0; // Commence à 0
    public float currentXP = 0f; // XP actuel (0.0 à 1.0 par niveau)
    
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
        
        // Vérifier les montées de niveau
        while (currentXP >= 1f && !IsMaxLevel)
        {
            currentXP -= 1f; // Reset la barre XP
            currentLevel++;
            Debug.Log($"[SimplePlayerXP] LEVEL UP! Now level {currentLevel}");
            
            if (IsMaxLevel)
            {
                currentXP = 0f; // Pas d'XP au niveau max
                Debug.Log("[SimplePlayerXP] MAX LEVEL REACHED!");
                break;
            }
        }
    }
    
    public float GetXPPercentage()
    {
        if (IsMaxLevel) return 1f;
        return currentXP; // currentXP va de 0.0 à 1.0
    }
    
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
    
    public string GetXPProgressText()
    {
        if (IsMaxLevel)
        {
            return "MAX LEVEL";
        }
        else
        {
            int currentXPDisplay = Mathf.FloorToInt(currentXP * 2); // 0, 1, ou 2 (pour 0, 0.5, 1.0)
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