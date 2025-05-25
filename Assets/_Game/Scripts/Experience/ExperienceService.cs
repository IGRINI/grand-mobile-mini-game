using System;
using UnityEngine;
using Zenject;

public class ExperienceService : IExperienceService, IInitializable, IDisposable
{
    private readonly ExperienceConfig _config;
    private readonly ExperienceSystem _experienceSystem;
    private readonly IEnemyHealthHandler _enemyHealthHandler;
    
    public ExperienceService(ExperienceConfig config, IEnemyHealthHandler enemyHealthHandler)
    {
        _config = config;
        _enemyHealthHandler = enemyHealthHandler;
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
        _experienceSystem.LevelChanged += OnLevelChanged;
        _experienceSystem.ExperienceChanged += OnExperienceChanged;
        _experienceSystem.LevelUp += OnLevelUp;
        _experienceSystem.ExperienceGained += OnExperienceGained;
        
        _enemyHealthHandler.EnemyDied += OnEnemyDied;
    }
    
    public void Dispose()
    {
        _experienceSystem.LevelChanged -= OnLevelChanged;
        _experienceSystem.ExperienceChanged -= OnExperienceChanged;
        _experienceSystem.LevelUp -= OnLevelUp;
        _experienceSystem.ExperienceGained -= OnExperienceGained;
        
        if (_enemyHealthHandler != null)
            _enemyHealthHandler.EnemyDied -= OnEnemyDied;
    }
    
    public void AddExperience(int amount, int enemyLevel = 1)
    {
        _experienceSystem.AddExperience(amount, enemyLevel);
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
    
    private void OnEnemyDied(Enemy enemy)
    {
        AddExperienceFromEnemy(enemy);
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