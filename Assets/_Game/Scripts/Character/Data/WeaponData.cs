using UnityEngine;

[CreateAssetMenu(menuName="Game/Weapon Data", fileName="WeaponData")]
public class WeaponData : ScriptableObject
{
    public string WeaponName;
    public GameObject WeaponPrefab;
    public GameObject ProjectilePrefab;
    public float Damage;
    public float FireRate;
    public float ProjectileSpeed;
} 