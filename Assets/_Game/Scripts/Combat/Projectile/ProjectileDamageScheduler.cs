using UnityEngine;
using System.Collections;
using Zenject;

public class ProjectileDamageScheduler : MonoBehaviour
{
    private object _target;
    private float _damage;
    private float _delay;
    private IHealthService _healthService;

    public void Construct(IHealthService healthService)
    {
        _healthService = healthService;
    }

    public void Initialize(object target, float damage, float delay)
    {
        _target = target;
        _damage = damage;
        _delay = delay;
        StartCoroutine(DoDamageAfterDelay());
    }

    private IEnumerator DoDamageAfterDelay()
    {
        yield return new WaitForSeconds(_delay);
        if (_target != null)
            _healthService.DamageEntity(_target, _damage);
        var poolable = GetComponent<PoolableProjectile>();
        if (poolable != null)
            poolable.ReturnToPool();
        else
            Destroy(gameObject);
    }
} 