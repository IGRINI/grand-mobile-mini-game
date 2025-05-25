using System;

public interface IExperienceService
{
    int CurrentLevel { get; }
    int CurrentExperience { get; }
    int TotalExperience { get; }
    int ExperienceToNextLevel { get; }
    float ExperienceProgress { get; }
    bool IsMaxLevel { get; }
    
    event Action<int> LevelChanged;
    event Action<int, int> ExperienceChanged;
    event Action<int> LevelUp;
    event Action<int, int> ExperienceGained;
    
    void AddExperience(int amount, int enemyLevel = 1);
    void AddExperienceFromEnemy(Enemy enemy);
    void SetLevel(int level);
    void SetExperience(int totalExperience);
    
    ExperienceSystemData GetSaveData();
    void LoadSaveData(ExperienceSystemData data);
} 