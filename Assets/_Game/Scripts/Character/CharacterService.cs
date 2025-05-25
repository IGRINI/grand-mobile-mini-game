using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using Object = UnityEngine.Object;

public class CharacterService : ICharacterService, IInitializable
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
            SeatCharacter(0, _selected);
            SeatCharacter(1, _selected);
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
            _healthService.RegisterEntity(character, character.Health);
            if (slotIndex > 0)
            {
                _teamHealthUI.AddPassenger(character, character.Name);
            }
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
        return character == _selected && _characters.IndexOf(character) == 0;
    }
} 