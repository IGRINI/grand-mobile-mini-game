using UnityEngine;
using Zenject;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    
    private IEnemyService _enemyService;
    private IHealthService _healthService;
    private float _damage;
    private float _creationTime;

    public float Lifetime => lifetime;
    public float CreationTime => _creationTime;
    public bool IsExpired => Time.time - _creationTime >= lifetime;

    [Inject]
    public void Construct(IEnemyService enemyService, IHealthService healthService)
    {
        _enemyService = enemyService;
        _healthService = healthService;
    }

    public void Initialize(float damage)
    {
        _damage = damage;
        _creationTime = Time.time;
    }

    public void ReturnToPool()
    {
        var poolable = GetComponent<PoolableProjectile>();
        if (poolable != null)
        {
            poolable.ReturnToPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Enemy FindEnemyByView(IEnemyView enemyView)
    {
        return _enemyService.GetEnemyByView(enemyView);
    }
} 