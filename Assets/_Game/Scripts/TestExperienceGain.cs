using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class TestExperienceGain : MonoBehaviour
{
    private IExperienceService _experienceService;
    private IUpgradeService _upgradeService;
    
    [Inject]
    public void Construct(IExperienceService experienceService, IUpgradeService upgradeService)
    {
        _experienceService = experienceService;
        _upgradeService = upgradeService;
    }
    
    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Тестируем добавление опыта...");
            _experienceService?.AddExperience(50);
        }
        
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            Debug.Log("Принудительно показываем апгрейды...");
            _upgradeService?.ShowUpgradeSelection();
        }
        
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            Debug.Log("Принудительно повышаем уровень...");
            _experienceService?.AddExperience(1000);
        }
    }
} 