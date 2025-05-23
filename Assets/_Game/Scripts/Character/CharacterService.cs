using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CharacterService : ICharacterService, IInitializable
{
    private readonly List<Character> _characters = new List<Character>();
    private Character _selected;
    private readonly ICarView _carView;

    public CharacterService(CharacterData[] configs, ICarView carView)
    {
        _carView = carView;
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
        var instance = Object.Instantiate(character.Prefab, pivot);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        var view = instance.GetComponent<ICharacterView>();
        view?.Initialize(character);
        var weaponPivot = view?.WeaponPivot;
        var weaponData = character.DefaultWeapon;
        if (weaponPivot != null && weaponData != null && weaponData.WeaponPrefab != null)
        {
            for (int i = weaponPivot.childCount - 1; i >= 0; i--)
                Object.Destroy(weaponPivot.GetChild(i).gameObject);
            var weaponInstance = Object.Instantiate(weaponData.WeaponPrefab, weaponPivot);
            weaponInstance.transform.localPosition = Vector3.zero;
            weaponInstance.transform.localRotation = Quaternion.identity;
            var wc = weaponInstance.GetComponent<IWeaponController>();
            wc?.Initialize(weaponData);
        }
    }
} 