using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using Object = UnityEngine.Object;

public class CharacterService : ICharacterService, IInitializable, IDisposable
{
    private readonly List<Character> _characters = new();
    private Character _selected;
    private readonly ICarView _carView;
    private readonly DiContainer _diContainer;
    private IHealthService _healthService;
    private TeamHealthUI _teamHealthUI;
    private readonly Dictionary<Character, ICharacterView> _characterViews = new();

    public CharacterService(CharacterData[] configs, ICarView carView, DiContainer container)
    {
        _carView = carView;
        _diContainer = container;
        foreach (var config in configs)
        {
            _characters.Add(new Character(config));
        }
    }

    [Inject]
    public void Construct(IHealthService healthService, TeamHealthUI teamHealthUI)
    {
        _healthService = healthService;
        _teamHealthUI = teamHealthUI;
    }

    public IReadOnlyList<Character> Characters => _characters;
    public Character Selected => _selected;

    public void Initialize()
    {
        if (_characters.Count > 0)
        {
            _selected = _characters[0];
            SeatCharacter(0, _characters[0]); // Водитель
            if (_characters.Count > 1)
            {
                SeatCharacter(1, _characters[1]); // Пассажир
            }
            else
            {
                // Создаем отдельный экземпляр персонажа для пассажира
                var passengerCharacter = new Character(_characters[0].Data);
                _characters.Add(passengerCharacter);
                SeatCharacter(1, passengerCharacter); // Пассажир
            }
        }
        
        // Подписываемся на смерть персонажей
        if (_healthService != null)
        {
            _healthService.EntityDied += OnEntityDied;
        }
    }

    public void Select(Character character)
    {
        if (!_characters.Contains(character)) return;
        int newIndex = _characters.IndexOf(character);
        _selected = character;
        SeatCharacter(newIndex, _selected);
    }

    private void SeatCharacter(int slotIndex, Character character)
    {
        var pivots = _carView.CharacterPivots;
        if (slotIndex < 0 || slotIndex >= pivots.Count) return;
        var pivot = pivots[slotIndex];
        for (int i = pivot.childCount - 1; i >= 0; i--)
            Object.Destroy(pivot.GetChild(i).gameObject);
        var instance = _diContainer.InstantiatePrefab(character.Prefab, pivot);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        var view = instance.GetComponent<ICharacterView>();
        view?.Initialize(character);
        view?.SetSeatIndex(slotIndex);
        
        _characterViews[character] = view;
        
        if (_healthService != null && _teamHealthUI != null)
        {
            Debug.Log($"CharacterService: Регистрируем персонажа {character.Name} в слоте {slotIndex}");
            _healthService.RegisterEntity(character, character.Health);
            if (slotIndex > 0)
            {
                Debug.Log($"Добавляем пассажира {character.Name} в TeamHealthUI");
                _teamHealthUI.AddPassenger(character, character.Name);
            }
            else
            {
                Debug.Log($"Персонаж {character.Name} - водитель, не добавляем в пассажиры");
            }
        }
        else
        {
            Debug.Log($"CharacterService: HealthService или TeamHealthUI равны null!");
        }
    }
    
    public IReadOnlyList<Character> GetAllCharacters()
    {
        return _characters;
    }
    
    public ICharacterView GetCharacterView(Character character)
    {
        return _characterViews.TryGetValue(character, out var view) ? view : null;
    }
    
    public bool IsDriver(Character character)
    {
        // Водитель - это персонаж в слоте 0
        var characterView = GetCharacterView(character);
        if (characterView != null)
        {
            return characterView.SeatIndex == 0;
        }
        return character == _selected;
    }
    
    private void OnEntityDied(object entity, IHealth health)
    {
        if (entity is Character character && _characters.Contains(character))
        {
            Debug.Log($"Персонаж {character.Name} умер!");
            
            // Убираем персонажа из UI
            if (_teamHealthUI != null)
            {
                _teamHealthUI.RemovePassenger(character);
            }
            
            // Убираем view персонажа
            if (_characterViews.TryGetValue(character, out var characterView))
            {
                if ((characterView as CharacterView)?.gameObject != null)
                {
                    Object.Destroy(((CharacterView)characterView).gameObject);
                }
                _characterViews.Remove(character);
            }
            
            // Убираем персонажа из списка
            _characters.Remove(character);
            
            // Если умер выбранный персонаж, выбираем другого
            if (_selected == character && _characters.Count > 0)
            {
                _selected = _characters[0];
            }
        }
    }
    
    public void Dispose()
    {
        if (_healthService != null)
        {
            _healthService.EntityDied -= OnEntityDied;
        }
    }
} 