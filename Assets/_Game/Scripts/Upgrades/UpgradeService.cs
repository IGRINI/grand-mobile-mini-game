using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class UpgradeService : IUpgradeService, IInitializable, IDisposable
{
    private readonly UpgradeData[] _availableUpgrades;
    private readonly List<PlayerUpgrade> _playerUpgrades = new();
    private readonly IExperienceService _experienceService;
    
    public UpgradeService(UpgradeData[] availableUpgrades, IExperienceService experienceService)
    {
        _availableUpgrades = availableUpgrades;
        _experienceService = experienceService;
    }
    
    public IReadOnlyList<PlayerUpgrade> PlayerUpgrades => _playerUpgrades;
    
    public event Action<UpgradeData[]> UpgradeOptionsAvailable;
    public event Action<UpgradeData> UpgradeSelected;
    public Action UpgradeSelectionClosed;
    
    public void Initialize()
    {
        Debug.Log("UpgradeService.Initialize() вызван!");
        _experienceService.LevelUp += OnLevelUp;
        Debug.Log("UpgradeService подписался на LevelUp события");
    }
    
    public void Dispose()
    {
        if (_experienceService != null)
            _experienceService.LevelUp -= OnLevelUp;
    }
    
    public void ShowUpgradeSelection()
    {
        Debug.Log($"UpgradeService.ShowUpgradeSelection() вызван! Уровень игрока: {_experienceService.CurrentLevel}");
        var options = GenerateUpgradeOptions(_experienceService.CurrentLevel);
        Debug.Log($"Сгенерировано {options.Length} вариантов апгрейдов");
        UpgradeOptionsAvailable?.Invoke(options);
        Debug.Log("Событие UpgradeOptionsAvailable вызвано");
    }
    
    public void SelectUpgrade(UpgradeData upgradeData)
    {
        if (!CanSelectUpgrade(upgradeData, _experienceService.CurrentLevel))
        {
            Debug.LogWarning($"Нельзя выбрать апгрейд: {upgradeData.upgradeName}");
            return;
        }
        
        var existingUpgrade = GetUpgrade(upgradeData.upgradeType);
        if (existingUpgrade != null)
        {
            existingUpgrade.Upgrade();
        }
        else
        {
            var newUpgrade = new PlayerUpgrade(upgradeData);
            newUpgrade.Upgrade();
            _playerUpgrades.Add(newUpgrade);
        }
        
        UpgradeSelected?.Invoke(upgradeData);
        UpgradeSelectionClosed?.Invoke();
    }
    
    public bool HasUpgrade(UpgradeType upgradeType)
    {
        return _playerUpgrades.Any(u => u.upgradeData.upgradeType == upgradeType && u.currentLevel > 0);
    }
    
    public PlayerUpgrade GetUpgrade(UpgradeType upgradeType)
    {
        return _playerUpgrades.FirstOrDefault(u => u.upgradeData.upgradeType == upgradeType);
    }
    
    public float GetUpgradeValue(UpgradeType upgradeType)
    {
        var upgrade = GetUpgrade(upgradeType);
        return upgrade?.CurrentValue ?? 0f;
    }
    
    public int GetUpgradeLevel(UpgradeType upgradeType)
    {
        var upgrade = GetUpgrade(upgradeType);
        return upgrade?.currentLevel ?? 0;
    }
    
    public UpgradeData[] GenerateUpgradeOptions(int playerLevel, int count = 3)
    {
        var availableOptions = new List<UpgradeData>();
        
        foreach (var upgradeData in _availableUpgrades)
        {
            if (CanSelectUpgrade(upgradeData, playerLevel))
            {
                availableOptions.Add(upgradeData);
            }
        }
        
        if (availableOptions.Count <= count)
        {
            return availableOptions.ToArray();
        }
        
        var selectedOptions = new List<UpgradeData>();
        var random = new System.Random();
        
        for (int i = 0; i < count && availableOptions.Count > 0; i++)
        {
            int randomIndex = random.Next(availableOptions.Count);
            selectedOptions.Add(availableOptions[randomIndex]);
            availableOptions.RemoveAt(randomIndex);
        }
        
        return selectedOptions.ToArray();
    }
    
    public bool CanSelectUpgrade(UpgradeData upgradeData, int playerLevel)
    {
        if (upgradeData == null) return false;
        
        if (playerLevel < upgradeData.minPlayerLevel || playerLevel > upgradeData.maxPlayerLevel)
            return false;
        
        var existingUpgrade = GetUpgrade(upgradeData.upgradeType);
        if (existingUpgrade != null && existingUpgrade.IsMaxLevel)
            return false;
        
        if (upgradeData.requiredUpgrades != null)
        {
            foreach (var required in upgradeData.requiredUpgrades)
            {
                if (!HasUpgrade(required))
                    return false;
            }
        }
        
        if (upgradeData.conflictingUpgrades != null)
        {
            foreach (var conflicting in upgradeData.conflictingUpgrades)
            {
                if (HasUpgrade(conflicting))
                    return false;
            }
        }
        
        return true;
    }
    
    private void OnLevelUp(int newLevel)
    {
        Debug.Log($"UpgradeService.OnLevelUp вызван! Новый уровень: {newLevel}");
        ShowUpgradeSelection();
    }
} 