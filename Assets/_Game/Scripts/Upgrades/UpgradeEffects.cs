using UnityEngine;

public static class UpgradeEffects
{
    private static IUpgradeEffectProvider _provider;
    
    public static void SetProvider(IUpgradeEffectProvider provider)
    {
        _provider = provider;
    }
    
    public static float GetFireRateMultiplier()
    {
        return _provider?.GetSpeedMultiplier(UpgradeType.IncreaseFireRate) ?? 1f;
    }
    
    public static float GetWeaponCooldownMultiplier()
    {
        return _provider?.GetWeaponCooldownMultiplier() ?? 1f;
    }
    
    public static float GetMovementSpeedMultiplier()
    {
        return _provider?.GetMovementSpeedMultiplier() ?? 1f;
    }
    
    public static float GetExperienceMultiplier()
    {
        return _provider?.GetExperienceMultiplier() ?? 1f;
    }
} 