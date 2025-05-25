using UnityEngine;

[CreateAssetMenu(menuName="Game/Experience Config", fileName="ExperienceConfig")]
public class ExperienceConfig : ScriptableObject
{
    [Header("Базовые настройки")]
    [SerializeField] private int baseExperienceRequired = 100;
    [SerializeField] private float exponentialMultiplier = 1.2f;
    [SerializeField] private float linearMultiplier = 50f;
    [SerializeField] private int maxLevel = 999;
    
    [Header("Бонусы за уровень")]
    [SerializeField] private float experienceMultiplierPerLevel = 0.05f;
    [SerializeField] private bool enableDiminishingReturns = true;
    [SerializeField] private float diminishingReturnsThreshold = 0.8f;
    
    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject experienceGainEffect;
    [SerializeField] private GameObject levelUpEffect;
    [SerializeField] private float effectDuration = 2f;
    
    public int BaseExperienceRequired => baseExperienceRequired;
    public float ExponentialMultiplier => exponentialMultiplier;
    public float LinearMultiplier => linearMultiplier;
    public int MaxLevel => maxLevel;
    public float ExperienceMultiplierPerLevel => experienceMultiplierPerLevel;
    public bool EnableDiminishingReturns => enableDiminishingReturns;
    public float DiminishingReturnsThreshold => diminishingReturnsThreshold;
    public GameObject ExperienceGainEffect => experienceGainEffect;
    public GameObject LevelUpEffect => levelUpEffect;
    public float EffectDuration => effectDuration;
    
    public int CalculateExperienceRequired(int level)
    {
        if (level <= 1) return 0;
        if (level > maxLevel) return int.MaxValue;
        
        float baseExp = baseExperienceRequired;
        float exponential = Mathf.Pow(exponentialMultiplier, level - 1);
        float linear = linearMultiplier * (level - 1);
        
        return Mathf.RoundToInt(baseExp * exponential + linear);
    }
    
    public int CalculateAdjustedExperience(int baseExperience, int playerLevel, int enemyLevel)
    {
        float adjustedExp = baseExperience;
        
        int levelDifference = enemyLevel - playerLevel;
        
        if (levelDifference > 0)
        {
            adjustedExp *= (1f + levelDifference * 0.1f);
        }
        else if (levelDifference < 0)
        {
            float penalty = Mathf.Abs(levelDifference) * 0.15f;
            if (enableDiminishingReturns && penalty > diminishingReturnsThreshold)
            {
                penalty = diminishingReturnsThreshold + (penalty - diminishingReturnsThreshold) * 0.5f;
            }
            adjustedExp *= Mathf.Max(0.1f, 1f - penalty);
        }
        
        float levelBonus = playerLevel * experienceMultiplierPerLevel;
        adjustedExp *= (1f + levelBonus);
        
        return Mathf.RoundToInt(adjustedExp);
    }
} 