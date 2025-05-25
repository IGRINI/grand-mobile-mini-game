using UnityEngine;

public class Enemy
{
    public EnemyData Data { get; }
    public IHealth Health { get; }

    public Enemy(EnemyData data)
    {
        Data = data;
        Health = new Health(data.Health);
    }

    public string Name => Data.EnemyName;
    public GameObject Prefab => Data.EnemyPrefab;
    public float MoveSpeed => Data.MoveSpeed;
    public float AttackDamage => Data.AttackDamage;
    public float AttackRange => Data.AttackRange;
    public float AttackCooldown => Data.AttackCooldown;
    public int RewardExperience => Data.RewardExperience;
    public WeaponData DefaultWeapon => Data.DefaultWeapon;
    public bool IsAlive => Health.IsAlive;
    public float CurrentHealth => Health.CurrentHealth;
} 