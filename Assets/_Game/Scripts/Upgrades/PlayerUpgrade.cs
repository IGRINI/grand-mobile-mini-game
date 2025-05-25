using System;

[Serializable]
public class PlayerUpgrade
{
    public UpgradeData upgradeData;
    public int currentLevel;
    
    public PlayerUpgrade(UpgradeData data)
    {
        upgradeData = data;
        currentLevel = 0;
    }
    
    public bool CanUpgrade => currentLevel < upgradeData.maxLevel;
    public bool IsMaxLevel => currentLevel >= upgradeData.maxLevel;
    public float CurrentValue => upgradeData.GetValueAtLevel(currentLevel);
    public float NextLevelValue => upgradeData.GetValueAtLevel(currentLevel + 1);
    public string CurrentDescription => upgradeData.GetFormattedDescription(currentLevel);
    public string NextLevelDescription => upgradeData.GetFormattedDescription(currentLevel + 1);
    
    public void Upgrade()
    {
        if (CanUpgrade)
        {
            currentLevel++;
        }
    }
} 