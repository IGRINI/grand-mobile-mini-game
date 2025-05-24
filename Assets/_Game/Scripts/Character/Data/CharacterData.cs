using UnityEngine;

[CreateAssetMenu(menuName="Game/Character Data", fileName="CharacterData")]
public class CharacterData : ScriptableObject
{
    public string CharacterName;
    public GameObject CharacterPrefab;
    public WeaponData DefaultWeapon;
    public float Health = 100f;
} 