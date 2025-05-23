using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public CharacterData Data { get; }

    public Character(CharacterData data)
    {
        Data = data;
    }

    public string Name => Data.CharacterName;
    public GameObject Prefab => Data.CharacterPrefab;
    public WeaponData DefaultWeapon => Data.DefaultWeapon;
} 