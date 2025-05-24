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

    private void OnTriggerEnter(Collider other)
    {
        var enemyView = other.GetComponent<IEnemyView>();
        if (enemyView != null)
        {
            var enemy = FindEnemyByView(enemyView);
            if (enemy != null)
            {
                _healthService.DamageEntity(enemy, _damage);
                ReturnToPool();
            }
            return;
        }
        
        var characterView = other.GetComponent<ICharacterView>();
        if (characterView != null)
        {
            var carController = other.GetComponentInParent<CarController>();
            if (carController != null)
            {
                _healthService.DamageEntity(carController, _damage);
                ReturnToPool();
            }
        }
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