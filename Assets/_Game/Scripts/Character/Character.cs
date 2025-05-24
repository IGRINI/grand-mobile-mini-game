using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public CharacterData Data { get; }
    public IHealth Health { get; }

    public Character(CharacterData data)
    {
        Data = data;
        Health = new Health(data.Health);
    }

    public string Name => Data.CharacterName;
    public GameObject Prefab => Data.CharacterPrefab;
    public WeaponData DefaultWeapon => Data.DefaultWeapon;
} 