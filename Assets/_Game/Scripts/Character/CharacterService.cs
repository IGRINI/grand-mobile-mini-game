using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CharacterService : ICharacterService, IInitializable
{
    private readonly List<Character> _characters = new();
    private Character _selected;
    private readonly ICarView _carView;
    private readonly DiContainer _diContainer;

    public CharacterService(CharacterData[] configs, ICarView carView, DiContainer container)
    {
        _carView = carView;
        _diContainer = container;
        foreach (var config in configs)
        {
            _characters.Add(new Character(config));
        }
    }

    public IReadOnlyList<Character> Characters => _characters;
    public Character Selected => _selected;

    public void Initialize()
    {
        if (_characters.Count > 0)
        {
            _selected = _characters[0];
            SeatCharacter(0, _selected);
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
    }
} 