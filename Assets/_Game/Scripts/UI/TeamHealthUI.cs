using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TeamHealthUI : MonoBehaviour
{
    [SerializeField] private CharacterHealthUI carHealthUI;
    [SerializeField] private List<CharacterHealthUI> passengerHealthUIs = new List<CharacterHealthUI>();
    
    private IHealthService _healthService;
    private CarController _carController;
    private Dictionary<object, int> _passengerSlots = new Dictionary<object, int>();
    private Dictionary<object, IHealth> _trackedHealths = new Dictionary<object, IHealth>();
    
    [Inject]
    public void Construct(IHealthService healthService, CarController carController)
    {
        _healthService = healthService;
        _carController = carController;
    }
    
    private void Start()
    {
        InitializeUI();
        SubscribeToCarHealth();
    }
    
    private void Awake()
    {
        // начальное скрытие UI пассажиров
        foreach (var ui in passengerHealthUIs)
        {
            ui.Hide();
        }
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromAllHealths();
    }
    
    private void InitializeUI()
    {
        if (carHealthUI != null)
        {
            carHealthUI.SetCharacterName("Машина");
            carHealthUI.Show();
        }
        
        // задаем имена для UI пассажиров, видимость управляется через Add/RemovePassenger
        for (int i = 0; i < passengerHealthUIs.Count; i++)
        {
            if (passengerHealthUIs[i] != null)
            {
                passengerHealthUIs[i].SetCharacterName($"Пассажир {i + 1}");
            }
        }
    }
    
    private void SubscribeToCarHealth()
    {
        if (_healthService == null || _carController == null) return;
        
        var carHealth = _healthService.GetHealth(_carController);
        if (carHealth != null && carHealthUI != null)
        {
            _trackedHealths[_carController] = carHealth;
            carHealth.HealthChanged += _ => OnCarHealthChanged(carHealth);
            carHealthUI.UpdateHealth(carHealth.CurrentHealth, carHealth.MaxHealth);
        }
    }
    
    private void OnCarHealthChanged(IHealth health)
    {
        if (carHealthUI != null)
        {
            carHealthUI.UpdateHealth(health.CurrentHealth, health.MaxHealth);
        }
    }
    
    private void OnPassengerHealthChanged(object passenger, IHealth health)
    {
        if (_passengerSlots.TryGetValue(passenger, out int slotIndex))
        {
            if (slotIndex >= 0 && slotIndex < passengerHealthUIs.Count)
            {
                passengerHealthUIs[slotIndex].UpdateHealth(health.CurrentHealth, health.MaxHealth);
            }
        }
    }
    
    private void UnsubscribeFromAllHealths()
    {
        foreach (var kvp in _trackedHealths)
        {
            var entity = kvp.Key;
            var health = kvp.Value;
            
            if (entity == _carController)
            {
                health.HealthChanged -= _ => OnCarHealthChanged(health);
            }
            else
            {
                health.HealthChanged -= _ => OnPassengerHealthChanged(entity, health);
            }
        }
        _trackedHealths.Clear();
    }
    
    public void AddPassenger(object passenger, string name = null)
    {
        if (_passengerSlots.ContainsKey(passenger)) return;
        
        for (int i = 0; i < passengerHealthUIs.Count; i++)
        {
            if (!passengerHealthUIs[i].gameObject.activeInHierarchy)
            {
                _passengerSlots[passenger] = i;
                passengerHealthUIs[i].SetCharacterName(name ?? $"Пассажир {i + 1}");
                passengerHealthUIs[i].Show();
                
                var passengerHealth = _healthService?.GetHealth(passenger);
                if (passengerHealth != null)
                {
                    _trackedHealths[passenger] = passengerHealth;
                    passengerHealth.HealthChanged += _ => OnPassengerHealthChanged(passenger, passengerHealth);
                    passengerHealthUIs[i].UpdateHealth(passengerHealth.CurrentHealth, passengerHealth.MaxHealth);
                }
                break;
            }
        }
    }
    
    public void RemovePassenger(object passenger)
    {
        if (_passengerSlots.TryGetValue(passenger, out int slotIndex))
        {
            if (slotIndex >= 0 && slotIndex < passengerHealthUIs.Count)
            {
                passengerHealthUIs[slotIndex].Hide();
            }
            _passengerSlots.Remove(passenger);
            
            if (_trackedHealths.TryGetValue(passenger, out var health))
            {
                health.HealthChanged -= _ => OnPassengerHealthChanged(passenger, health);
                _trackedHealths.Remove(passenger);
            }
        }
    }
} 