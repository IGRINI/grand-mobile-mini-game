using UnityEngine;

[CreateAssetMenu(menuName="Game/Enemy Data", fileName="EnemyData")]
public class EnemyData : ScriptableObject
{
    public string EnemyName;
    public GameObject EnemyPrefab;
    public float Health;
    public float MoveSpeed;
    public float AttackDamage;
    public float AttackRange;
    public float AttackCooldown;
    public int RewardExperience;
    public WeaponData DefaultWeapon;
} 