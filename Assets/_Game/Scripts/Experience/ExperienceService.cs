using System;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public class ExperienceService : IExperienceService, IInitializable, IDisposable
{
    private readonly ExperienceConfig _config;
    private readonly ExperienceSystem _experienceSystem;
    private readonly IHealthService _healthService;
    
    public ExperienceService(ExperienceConfig config, IHealthService healthService)
    {
        _config = config;
        _healthService = healthService;
        _experienceSystem = new ExperienceSystem(config);
    }
    
    public int CurrentLevel => _experienceSystem.CurrentLevel;
    public int CurrentExperience => _experienceSystem.CurrentExperience;
    public int TotalExperience => _experienceSystem.TotalExperience;
    public int ExperienceToNextLevel => _experienceSystem.ExperienceToNextLevel;
    public float ExperienceProgress => _experienceSystem.ExperienceProgress;
    public bool IsMaxLevel => _experienceSystem.IsMaxLevel;
    
    public event Action<int> LevelChanged;
    public event Action<int, int> ExperienceChanged;
    public event Action<int> LevelUp;
    public event Action<int, int> ExperienceGained;
    
    public void Initialize()
    {
        Debug.Log("ExperienceService.Initialize() вызван!");
        _experienceSystem.LevelChanged += OnLevelChanged;
        _experienceSystem.ExperienceChanged += OnExperienceChanged;
        _experienceSystem.LevelUp += OnLevelUp;
        _experienceSystem.ExperienceGained += OnExperienceGained;
        
        _healthService.EntityDied += OnEntityDied;
        Debug.Log("ExperienceService подписался на EntityDied события");
    }
    
    public void Dispose()
    {
        _experienceSystem.LevelChanged -= OnLevelChanged;
        _experienceSystem.ExperienceChanged -= OnExperienceChanged;
        _experienceSystem.LevelUp -= OnLevelUp;
        _experienceSystem.ExperienceGained -= OnExperienceGained;
        
        if (_healthService != null)
            _healthService.EntityDied -= OnEntityDied;
    }
    
    public void AddExperience(int amount, int enemyLevel = 1)
    {
        Debug.Log($"ExperienceService.AddExperience вызван: amount={amount}, enemyLevel={enemyLevel}");
        float multiplier = UpgradeEffects.GetExperienceMultiplier();
        int adjustedAmount = Mathf.RoundToInt(amount * multiplier);
        Debug.Log($"Множитель опыта: {multiplier}, скорректированный опыт: {adjustedAmount}");
        _experienceSystem.AddExperience(adjustedAmount, enemyLevel);
    }
    
    public void AddExperienceFromEnemy(Enemy enemy)
    {
        if (enemy?.RewardExperience > 0)
        {
            int enemyLevel = CalculateEnemyLevel(enemy);
            AddExperience(enemy.RewardExperience, enemyLevel);
        }
    }
    
    public void SetLevel(int level)
    {
        _experienceSystem.SetLevel(level);
    }
    
    public void SetExperience(int totalExperience)
    {
        _experienceSystem.SetExperience(totalExperience);
    }
    
    public ExperienceSystemData GetSaveData()
    {
        return _experienceSystem.GetSaveData();
    }
    
    public void LoadSaveData(ExperienceSystemData data)
    {
        _experienceSystem.LoadSaveData(data);
    }
    
    private void OnEntityDied(object entity, IHealth health)
    {
        if (entity is Enemy enemy)
        {
            Debug.Log($"Враг {enemy.Name} умер! Начисляем {enemy.RewardExperience} опыта");
            AddExperienceFromEnemy(enemy);
        }
    }
    
    private void OnLevelChanged(int newLevel)
    {
        LevelChanged?.Invoke(newLevel);
    }
    
    private void OnExperienceChanged(int currentExp, int requiredExp)
    {
        ExperienceChanged?.Invoke(currentExp, requiredExp);
    }
    
    private void OnLevelUp(int newLevel)
    {
        ShowLevelUpEffect();
        LevelUp?.Invoke(newLevel);
    }
    
    private void OnExperienceGained(int adjustedAmount, int originalAmount)
    {
        ShowExperienceGainEffect(adjustedAmount);
        ExperienceGained?.Invoke(adjustedAmount, originalAmount);
    }
    
    private void ShowExperienceGainEffect(int amount)
    {
        if (_config.ExperienceGainEffect != null)
        {
            var effect = Object.Instantiate(_config.ExperienceGainEffect);
            Object.Destroy(effect, _config.EffectDuration);
        }
    }
    
    private void ShowLevelUpEffect()
    {
        if (_config.LevelUpEffect != null)
        {
            var effect = Object.Instantiate(_config.LevelUpEffect);
            Object.Destroy(effect, _config.EffectDuration);
        }
    }
    
    private int CalculateEnemyLevel(Enemy enemy)
    {
        float healthRatio = enemy.Data.Health / 100f;
        float damageRatio = enemy.Data.AttackDamage / 50f;
        float speedRatio = enemy.Data.MoveSpeed / 5f;
        
        float averageRatio = (healthRatio + damageRatio + speedRatio) / 3f;
        int estimatedLevel = Mathf.RoundToInt(averageRatio * 10f);
        
        return Mathf.Max(1, estimatedLevel);
    }
} 