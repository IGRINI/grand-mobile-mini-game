using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class UpgradeEffectApplier : IInitializable, System.IDisposable, IUpgradeEffectProvider
{
    private readonly IUpgradeService _upgradeService;
    private readonly IHealthService _healthService;
    private readonly ICharacterService _characterService;
    private readonly IExperienceService _experienceService;
    
    private readonly Dictionary<object, float> _baseMaxSpeeds = new();
    private readonly Dictionary<object, float> _baseFireRates = new();
    private readonly Dictionary<object, float> _baseRanges = new();
    private readonly Dictionary<object, float> _baseDamages = new();
    
    private object _carController; // Ссылка на CarController
    private ICarModel _carModel; // Получаем через CarController
    private float _healthRegenerationTimer;
    private const float HEALTH_REGEN_INTERVAL = 1f;
    
    public UpgradeEffectApplier(
        IUpgradeService upgradeService,
        IHealthService healthService,
        ICharacterService characterService,
        IExperienceService experienceService)
    {
        _upgradeService = upgradeService;
        _healthService = healthService;
        _characterService = characterService;
        _experienceService = experienceService;
    }
    
    public void Initialize()
    {
        _upgradeService.UpgradeSelected += OnUpgradeSelected;
        _experienceService.ExperienceGained += OnExperienceGained;
        _healthService.EntityRegistered += OnEntityRegistered;
        StoreBaseValues();
        UpgradeEffects.SetProvider(this);
    }
    
    public void Dispose()
    {
        if (_upgradeService != null)
            _upgradeService.UpgradeSelected -= OnUpgradeSelected;
        if (_experienceService != null)
            _experienceService.ExperienceGained -= OnExperienceGained;
        if (_healthService != null)
            _healthService.EntityRegistered -= OnEntityRegistered;
    }
    
    private void StoreBaseValues()
    {
        var characters = _characterService.GetAllCharacters();
        foreach (var character in characters)
        {
            if (character.DefaultWeapon != null)
            {
                _baseFireRates[character] = character.DefaultWeapon.FireRate;
                _baseRanges[character] = character.DefaultWeapon.Range;
                _baseDamages[character] = character.DefaultWeapon.Damage;
            }
        }
    }
    
    private void StoreCarBaseValues()
    {
        if (_carModel != null)
        {
            _baseMaxSpeeds[_carModel] = _carModel.MaxSpeed;
        }
    }
    
    private void OnUpgradeSelected(UpgradeData upgradeData)
    {
        ApplyUpgradeEffect(upgradeData);
    }
    
    private void OnExperienceGained(int adjustedAmount, int originalAmount)
    {
        // Опыт уже модифицируется в ExperienceService через UpgradeEffects
    }
    
    private void OnEntityRegistered(object entity, IHealth health)
    {
        // Ищем CarController среди зарегистрированных сущностей
        if (entity is ICarModelProvider carModelProvider)
        {
            _carController = entity;
            _carModel = carModelProvider.CarModel;
            StoreCarBaseValues();
            
            // Устанавливаем модификатор урона для машины
            health.SetDamageModifier(damage => 
            {
                float reduction = GetIncomingDamageReduction();
                float armorValue = GetArmorValue();
                float totalReduction = reduction + (armorValue / 100f);
                return damage * (1f - Mathf.Clamp01(totalReduction));
            });
        }
        else if (entity is Character character)
        {
            // Устанавливаем модификатор урона для персонажей
                            health.SetDamageModifier(damage => 
                {
                    float characterDamageReduction = GetCharacterDamageReduction();
                    float generalReduction = GetIncomingDamageReduction();
                    float armorValue = GetArmorValue();
                    float totalReduction = characterDamageReduction + generalReduction + (armorValue / 100f);
                    float modifiedDamage = damage * (1f - Mathf.Clamp01(totalReduction));
                    Debug.Log($"Обновление модификатора урона для {character.Name}: {damage:F1} -> {modifiedDamage:F1} (защита: {totalReduction * 100f:F1}%)");
                    return modifiedDamage;
                });
        }
    }
    
    private void ApplyUpgradeEffect(UpgradeData upgradeData)
    {
        var upgrade = _upgradeService.GetUpgrade(upgradeData.upgradeType);
        if (upgrade == null) return;
        
        switch (upgradeData.upgradeType)
        {
            case UpgradeType.HealthRestore:
                ApplyHealthRestore(upgrade.CurrentValue);
                break;
                
            case UpgradeType.AddPassenger:
                ApplyAddPassenger();
                break;
                
            case UpgradeType.ReduceIncomingDamage:
                ApplyDamageReductionUpgrade();
                Debug.Log($"Уменьшение входящего урона на {upgrade.CurrentValue}%");
                break;
                
            case UpgradeType.IncreaseCharacterDamage:
                ApplyCharacterDamageIncrease();
                break;
                
            case UpgradeType.ReduceDamageToCharacters:
                ApplyCharacterDamageReductionUpgrade();
                Debug.Log($"Уменьшение урона по персонажам на {upgrade.CurrentValue}%");
                break;
                
            case UpgradeType.IncreaseRamDamage:
                Debug.Log($"Увеличение урона от сбивания на {upgrade.CurrentValue}%");
                break;
                
            case UpgradeType.IncreaseMaxSpeed:
                ApplyMaxSpeedIncrease();
                break;
                
            case UpgradeType.IncreaseFireRate:
                ApplyFireRateIncrease();
                break;
                
            case UpgradeType.IncreaseRange:
                ApplyRangeIncrease();
                break;
                
            case UpgradeType.IncreaseExperienceGain:
                Debug.Log($"Увеличение получения опыта на {upgrade.CurrentValue}%");
                break;
                
            case UpgradeType.IncreaseHealthRegeneration:
                Debug.Log($"Увеличение регенерации здоровья на {upgrade.CurrentValue}/сек");
                break;
                
            case UpgradeType.ReduceWeaponCooldown:
                ApplyWeaponCooldownReduction();
                break;
                
            case UpgradeType.IncreaseMovementSpeed:
                Debug.Log($"Увеличение скорости движения на {upgrade.CurrentValue}%");
                break;
                
            case UpgradeType.AddArmor:
                ApplyArmorUpgrade();
                Debug.Log($"Добавление брони: {upgrade.CurrentValue}");
                break;
                
            case UpgradeType.IncreaseCriticalChance:
                Debug.Log($"Увеличение шанса критического удара на {upgrade.CurrentValue}%");
                break;
                
            case UpgradeType.IncreaseCriticalDamage:
                Debug.Log($"Увеличение критического урона на {upgrade.CurrentValue}%");
                break;
        }
    }
    
    private void ApplyHealthRestore(float amount)
    {
        var characters = _characterService.GetAllCharacters();
        foreach (var character in characters)
        {
            if (character.Health.IsAlive)
            {
                character.Health.Heal(amount);
                Debug.Log($"Восстановлено {amount} здоровья для {character.Name}");
            }
        }
        
        if (_carController != null)
        {
            _healthService.HealEntity(_carController, amount);
            Debug.Log($"Восстановлено {amount} здоровья для машины");
        }
    }
    
    private void ApplyAddPassenger()
    {
        Debug.Log("Добавлен новый пассажир! (Требует интеграцию с системой персонажей)");
    }
    
    private void ApplyCharacterDamageIncrease()
    {
        var characters = _characterService.GetAllCharacters();
        foreach (var character in characters)
        {
            if (character.DefaultWeapon != null && _baseDamages.ContainsKey(character))
            {
                var multiplier = GetDamageMultiplier(UpgradeType.IncreaseCharacterDamage);
                var newDamage = _baseDamages[character] * multiplier;
                Debug.Log($"Урон {character.Name} увеличен до {newDamage:F1}");
            }
        }
    }
    
    private void ApplyMaxSpeedIncrease()
    {
        if (_carModel != null && _baseMaxSpeeds.ContainsKey(_carModel))
        {
            var multiplier = GetSpeedMultiplier(UpgradeType.IncreaseMaxSpeed);
            var newMaxSpeed = _baseMaxSpeeds[_carModel] * multiplier;
            Debug.Log($"Максимальная скорость увеличена до {newMaxSpeed:F1}");
        }
    }
    
    private void ApplyFireRateIncrease()
    {
        var characters = _characterService.GetAllCharacters();
        foreach (var character in characters)
        {
            if (character.DefaultWeapon != null && _baseFireRates.ContainsKey(character))
            {
                var multiplier = GetSpeedMultiplier(UpgradeType.IncreaseFireRate);
                var newFireRate = _baseFireRates[character] * multiplier;
                Debug.Log($"Скорость стрельбы {character.Name} увеличена до {newFireRate:F1}");
            }
        }
    }
    
    private void ApplyRangeIncrease()
    {
        var characters = _characterService.GetAllCharacters();
        foreach (var character in characters)
        {
            if (character.DefaultWeapon != null && _baseRanges.ContainsKey(character))
            {
                var multiplier = GetSpeedMultiplier(UpgradeType.IncreaseRange);
                var newRange = _baseRanges[character] * multiplier;
                Debug.Log($"Дальность атаки {character.Name} увеличена до {newRange:F1}");
            }
        }
    }
    
    private void ApplyWeaponCooldownReduction()
    {
        var reductionPercent = _upgradeService.GetUpgradeValue(UpgradeType.ReduceWeaponCooldown);
        Debug.Log($"Перезарядка оружия уменьшена на {reductionPercent}%");
    }
    
    private void ApplyDamageReductionUpgrade()
    {
        UpdateAllDamageModifiers();
    }
    
    private void ApplyCharacterDamageReductionUpgrade()
    {
        UpdateCharacterDamageModifiers();
    }
    
    private void ApplyArmorUpgrade()
    {
        UpdateAllDamageModifiers();
    }
    
    private void UpdateAllDamageModifiers()
    {
        UpdateCharacterDamageModifiers();
        UpdateCarDamageModifier();
    }
    
    private void UpdateCharacterDamageModifiers()
    {
        var characters = _characterService.GetAllCharacters();
        foreach (var character in characters)
        {
            var health = _healthService.GetHealth(character);
            if (health != null)
            {
                health.SetDamageModifier(damage => 
                {
                    float characterDamageReduction = GetCharacterDamageReduction();
                    float generalReduction = GetIncomingDamageReduction();
                    float armorValue = GetArmorValue();
                    float totalReduction = characterDamageReduction + generalReduction + (armorValue / 100f);
                    float modifiedDamage = damage * (1f - Mathf.Clamp01(totalReduction));
                    Debug.Log($"Персонаж {character.Name} получает урон: {damage:F1} -> {modifiedDamage:F1} (защита: {totalReduction * 100f:F1}%)");
                    return modifiedDamage;
                });
            }
        }
    }
    
    private void UpdateCarDamageModifier()
    {
        if (_carController != null)
        {
            var health = _healthService.GetHealth(_carController);
            if (health != null)
            {
                health.SetDamageModifier(damage => 
                {
                    float reduction = GetIncomingDamageReduction();
                    float armorValue = GetArmorValue();
                    float totalReduction = reduction + (armorValue / 100f);
                    return damage * (1f - Mathf.Clamp01(totalReduction));
                });
            }
        }
    }
    
    public void Update()
    {
        UpdateHealthRegeneration();
    }
    
    private void UpdateHealthRegeneration()
    {
        var regenValue = _upgradeService.GetUpgradeValue(UpgradeType.IncreaseHealthRegeneration);
        if (regenValue <= 0) return;
        
        _healthRegenerationTimer += Time.deltaTime;
        if (_healthRegenerationTimer >= HEALTH_REGEN_INTERVAL)
        {
            _healthRegenerationTimer = 0f;
            
            var characters = _characterService.GetAllCharacters();
            foreach (var character in characters)
            {
                if (character.Health.IsAlive && character.Health.CurrentHealth < character.Health.MaxHealth)
                {
                    character.Health.Heal(regenValue);
                }
            }
            
            if (_carController != null)
            {
                var carHealth = _healthService.GetHealth(_carController);
                if (carHealth != null && carHealth.IsAlive && carHealth.CurrentHealth < carHealth.MaxHealth)
                {
                    carHealth.Heal(regenValue);
                }
            }
        }
    }
    
    public float GetDamageMultiplier(UpgradeType upgradeType)
    {
        var value = _upgradeService.GetUpgradeValue(upgradeType);
        return 1f + (value / 100f);
    }
    
    public float GetDamageReduction(UpgradeType upgradeType)
    {
        var value = _upgradeService.GetUpgradeValue(upgradeType);
        return value / 100f;
    }
    
    public float GetSpeedMultiplier(UpgradeType upgradeType)
    {
        var value = _upgradeService.GetUpgradeValue(upgradeType);
        return 1f + (value / 100f);
    }
    
    public float GetExperienceMultiplier()
    {
        var value = _upgradeService.GetUpgradeValue(UpgradeType.IncreaseExperienceGain);
        return 1f + (value / 100f);
    }
    
    public float GetCriticalChance()
    {
        return _upgradeService.GetUpgradeValue(UpgradeType.IncreaseCriticalChance);
    }
    
    public float GetCriticalDamageMultiplier()
    {
        var value = _upgradeService.GetUpgradeValue(UpgradeType.IncreaseCriticalDamage);
        return 1f + (value / 100f);
    }
    
    public float GetArmorValue()
    {
        return _upgradeService.GetUpgradeValue(UpgradeType.AddArmor);
    }
    
    public float GetWeaponCooldownMultiplier()
    {
        var reduction = _upgradeService.GetUpgradeValue(UpgradeType.ReduceWeaponCooldown);
        return 1f - (reduction / 100f);
    }
    
    public float GetMovementSpeedMultiplier()
    {
        var value = _upgradeService.GetUpgradeValue(UpgradeType.IncreaseMovementSpeed);
        return 1f + (value / 100f);
    }
    
    public float GetRamDamageMultiplier()
    {
        var value = _upgradeService.GetUpgradeValue(UpgradeType.IncreaseRamDamage);
        return 1f + (value / 100f);
    }
    
    public float GetIncomingDamageReduction()
    {
        var value = _upgradeService.GetUpgradeValue(UpgradeType.ReduceIncomingDamage);
        return value / 100f;
    }
    
    public float GetCharacterDamageReduction()
    {
        var value = _upgradeService.GetUpgradeValue(UpgradeType.ReduceDamageToCharacters);
        return value / 100f;
    }
    
} 