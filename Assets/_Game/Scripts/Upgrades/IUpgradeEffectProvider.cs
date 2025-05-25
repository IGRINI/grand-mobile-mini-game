public interface IUpgradeEffectProvider
{
    float GetDamageMultiplier(UpgradeType upgradeType);
    float GetDamageReduction(UpgradeType upgradeType);
    float GetSpeedMultiplier(UpgradeType upgradeType);
    float GetExperienceMultiplier();
    float GetCriticalChance();
    float GetCriticalDamageMultiplier();
    float GetArmorValue();
    float GetWeaponCooldownMultiplier();
    float GetMovementSpeedMultiplier();
    float GetRamDamageMultiplier();
    float GetIncomingDamageReduction();
    float GetCharacterDamageReduction();
} 