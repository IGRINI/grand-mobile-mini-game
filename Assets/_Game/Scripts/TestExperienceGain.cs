using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class TestExperienceGain : MonoBehaviour
{
    private IExperienceService _experienceService;
    
    [Inject]
    public void Construct(IExperienceService experienceService)
    {
        _experienceService = experienceService;
    }
    
    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Тестируем добавление опыта...");
            _experienceService?.AddExperience(50);
        }
    }
} 