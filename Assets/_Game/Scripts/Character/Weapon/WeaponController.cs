using UnityEngine;

public class WeaponController : MonoBehaviour, IWeaponController
{
    private WeaponData _config;
    private float _timer;

    public void Initialize(WeaponData config)
    {
        _config = config;
        _timer = 0f;
    }

    private void Update()
    {
        if (_config == null || _config.ProjectilePrefab == null) return;

        _timer += Time.deltaTime;
        float interval = 1f / _config.FireRate;
        if (_timer >= interval)
        {
            _timer -= interval;
            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        var proj = Instantiate(_config.ProjectilePrefab, transform.position, transform.rotation);
        var rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = transform.forward * _config.ProjectileSpeed;
    }
} 