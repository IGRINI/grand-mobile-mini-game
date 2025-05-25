using System;
using System.Collections.Generic;

public interface IUpgradeService
{
    IReadOnlyList<PlayerUpgrade> PlayerUpgrades { get; }
    
    event Action<UpgradeData[]> UpgradeOptionsAvailable;
    event Action<UpgradeData> UpgradeSelected;
    
    void ShowUpgradeSelection();
    void SelectUpgrade(UpgradeData upgradeData);
    bool HasUpgrade(UpgradeType upgradeType);
    PlayerUpgrade GetUpgrade(UpgradeType upgradeType);
    float GetUpgradeValue(UpgradeType upgradeType);
    int GetUpgradeLevel(UpgradeType upgradeType);
    
    UpgradeData[] GenerateUpgradeOptions(int playerLevel, int count = 3);
    bool CanSelectUpgrade(UpgradeData upgradeData, int playerLevel);
} 