using System;

public class ExperienceSystem
{
    private readonly ExperienceConfig _config;
    private int _currentLevel = 1;
    private int _currentExperience = 0;
    private int _totalExperience = 0;
    
    public event Action<int> LevelChanged;
    public event Action<int, int> ExperienceChanged;
    public event Action<int> LevelUp;
    public event Action<int, int> ExperienceGained;
    
    public ExperienceSystem(ExperienceConfig config)
    {
        _config = config;
    }
    
    public int CurrentLevel => _currentLevel;
    public int CurrentExperience => _currentExperience;
    public int TotalExperience => _totalExperience;
    public int ExperienceToNextLevel => GetExperienceRequiredForLevel(_currentLevel + 1) - _currentExperience;
    public float ExperienceProgress => GetExperienceProgress();
    public bool IsMaxLevel => _currentLevel >= _config.MaxLevel;
    
    public void AddExperience(int amount, int enemyLevel = 1)
    {
        if (IsMaxLevel || amount <= 0) return;
        
        int adjustedAmount = _config.CalculateAdjustedExperience(amount, _currentLevel, enemyLevel);
        
        _currentExperience += adjustedAmount;
        _totalExperience += adjustedAmount;
        
        ExperienceGained?.Invoke(adjustedAmount, amount);
        
        CheckLevelUp();
        
        ExperienceChanged?.Invoke(_currentExperience, GetExperienceRequiredForLevel(_currentLevel + 1));
    }
    
    public void SetLevel(int level)
    {
        if (level < 1 || level > _config.MaxLevel) return;
        
        int oldLevel = _currentLevel;
        _currentLevel = level;
        
        if (level == 1)
        {
            _currentExperience = 0;
            _totalExperience = 0;
        }
        else
        {
            _currentExperience = GetExperienceRequiredForLevel(level);
            _totalExperience = _currentExperience;
        }
        
        if (oldLevel != _currentLevel)
        {
            LevelChanged?.Invoke(_currentLevel);
        }
        
        ExperienceChanged?.Invoke(_currentExperience, GetExperienceRequiredForLevel(_currentLevel + 1));
    }
    
    public void SetExperience(int totalExperience)
    {
        if (totalExperience < 0) return;
        
        _totalExperience = totalExperience;
        _currentExperience = totalExperience;
        
        int newLevel = CalculateLevelFromExperience(totalExperience);
        int oldLevel = _currentLevel;
        
        _currentLevel = newLevel;
        
        if (oldLevel != _currentLevel)
        {
            LevelChanged?.Invoke(_currentLevel);
        }
        
        ExperienceChanged?.Invoke(_currentExperience, GetExperienceRequiredForLevel(_currentLevel + 1));
    }
    
    private void CheckLevelUp()
    {
        int newLevel = CalculateLevelFromExperience(_currentExperience);
        
        if (newLevel > _currentLevel)
        {
            int oldLevel = _currentLevel;
            _currentLevel = newLevel;
            
            UnityEngine.Debug.Log($"ExperienceSystem: Повышение уровня! {oldLevel} -> {newLevel}");
            LevelUp?.Invoke(_currentLevel);
            LevelChanged?.Invoke(_currentLevel);
        }
    }
    
    private int CalculateLevelFromExperience(int experience)
    {
        for (int level = 1; level <= _config.MaxLevel; level++)
        {
            int requiredExp = GetExperienceRequiredForLevel(level + 1);
            if (experience < requiredExp)
            {
                return level;
            }
        }
        return _config.MaxLevel;
    }
    
    private int GetExperienceRequiredForLevel(int level)
    {
        return _config.CalculateExperienceRequired(level);
    }
    
    private float GetExperienceProgress()
    {
        if (IsMaxLevel) return 1f;
        
        int currentLevelExp = GetExperienceRequiredForLevel(_currentLevel);
        int nextLevelExp = GetExperienceRequiredForLevel(_currentLevel + 1);
        int expInCurrentLevel = _currentExperience - currentLevelExp;
        int expNeededForLevel = nextLevelExp - currentLevelExp;
        
        return expNeededForLevel > 0 ? (float)expInCurrentLevel / expNeededForLevel : 0f;
    }
    
    public ExperienceSystemData GetSaveData()
    {
        return new ExperienceSystemData
        {
            CurrentLevel = _currentLevel,
            TotalExperience = _totalExperience
        };
    }
    
    public void LoadSaveData(ExperienceSystemData data)
    {
        SetLevel(data.CurrentLevel);
        SetExperience(data.TotalExperience);
    }
}

[System.Serializable]
public class ExperienceSystemData
{
    public int CurrentLevel;
    public int TotalExperience;
} 