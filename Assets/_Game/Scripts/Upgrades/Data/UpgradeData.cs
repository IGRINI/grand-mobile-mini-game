using UnityEngine;
using System;

[Serializable]
public class UpgradeData
{
    public string upgradeName;
    public string description;
    public UpgradeType upgradeType;
    
    public float value;
    public bool isPercentage;
    public int maxLevel = 5;
    public float valuePerLevel = 1f;
    
    public int minPlayerLevel = 1;
    public int maxPlayerLevel = 999;
    public UpgradeType[] requiredUpgrades;
    public UpgradeType[] conflictingUpgrades;
    
    public Color cardColor = Color.white;
    public string formattedDescription;
    
    public UpgradeData(
        string name,
        string desc,
        UpgradeType type,
        float val,
        bool percentage = false,
        int maxLvl = 5,
        float valuePerLvl = 1f,
        int minLvl = 1,
        int maxPlayerLvl = 999,
        string formatDesc = null,
        Color? color = null,
        UpgradeType[] required = null,
        UpgradeType[] conflicting = null)
    {
        upgradeName = name;
        description = desc;
        upgradeType = type;
        value = val;
        isPercentage = percentage;
        maxLevel = maxLvl;
        valuePerLevel = valuePerLvl;
        minPlayerLevel = minLvl;
        maxPlayerLevel = maxPlayerLvl;
        formattedDescription = formatDesc ?? desc;
        cardColor = color ?? Color.white;
        requiredUpgrades = required ?? new UpgradeType[0];
        conflictingUpgrades = conflicting ?? new UpgradeType[0];
    }
    
    public float GetValueAtLevel(int level)
    {
        return value + (valuePerLevel * (level - 1));
    }
    
    public string GetFormattedDescription(int level)
    {
        if (!string.IsNullOrEmpty(formattedDescription))
        {
            float currentValue = GetValueAtLevel(level);
            string valueText = isPercentage ? $"{currentValue:F0}%" : currentValue.ToString("F1");
            return formattedDescription.Replace("{value}", valueText);
        }
        return description;
    }
}

public enum UpgradeType
{
    HealthRestore,
    AddPassenger,
    ReduceIncomingDamage,
    IncreaseCharacterDamage,
    ReduceDamageToCharacters,
    IncreaseRamDamage,
    IncreaseMaxSpeed,
    IncreaseFireRate,
    IncreaseRange,
    IncreaseExperienceGain,
    IncreaseHealthRegeneration,
    ReduceWeaponCooldown,
    IncreaseMovementSpeed,
    AddArmor,
    IncreaseCriticalChance,
    IncreaseCriticalDamage
} 