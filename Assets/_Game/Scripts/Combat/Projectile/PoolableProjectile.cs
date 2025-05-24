using UnityEngine;

public class PoolableProjectile : MonoBehaviour
{
    private ProjectilePool _pool;
    private string _prefabKey;

    public string PrefabKey => _prefabKey;

    public void SetPool(ProjectilePool pool)
    {
        _pool = pool;
    }

    public void SetPrefabKey(string key)
    {
        _prefabKey = key;
    }

    public void ReturnToPool()
    {
        if (_pool != null)
        {
            _pool.ReturnProjectile(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
} 