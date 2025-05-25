using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Vector3 spawnBoundsMin = new Vector3(-10f, 0f, -10f);
    [SerializeField] private Vector3 spawnBoundsMax = new Vector3(10f, 0f, 10f);
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxActiveEnemies = 10;
    private EnemyData[] _enemyConfigs;
    
    private IEnemyService _enemyService;
    private ICarView _carView;
    private float _spawnTimer;

    [Inject]
    public void Construct(EnemyData[] enemyConfigs, IEnemyService enemyService, ICarView carView)
    {
        _enemyConfigs = enemyConfigs;
        _enemyService = enemyService;
        _carView = carView;
    }

    private void Update()
    {
        _spawnTimer += Time.deltaTime;
        
        if (_spawnTimer >= spawnInterval && _enemyService.ActiveEnemies.Count < maxActiveEnemies)
        {
            _spawnTimer = 0f;
            SpawnRandomEnemy();
        }
    }

    private void SpawnRandomEnemy()
    {
        if (_enemyConfigs == null || _enemyConfigs.Length == 0 || _carView?.Transform == null) return;
        
        var randomConfig = _enemyConfigs[Random.Range(0, _enemyConfigs.Length)];
        var spawnPosition = GetRandomSpawnPosition();
        
        _enemyService.SpawnEnemy(randomConfig, spawnPosition);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(spawnBoundsMin.x, spawnBoundsMax.x);
        float y = spawnBoundsMin.y;
        float z = Random.Range(spawnBoundsMin.z, spawnBoundsMax.z);
        Vector3 localPos = new Vector3(x, y, z);
        return transform.TransformPoint(localPos);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 centerLocal = (spawnBoundsMin + spawnBoundsMax) * 0.5f;
        Vector3 size = new Vector3(
            spawnBoundsMax.x - spawnBoundsMin.x,
            spawnBoundsMax.y - spawnBoundsMin.y,
            spawnBoundsMax.z - spawnBoundsMin.z
        );
        Gizmos.DrawWireCube(centerLocal, size);
        Gizmos.matrix = oldMatrix;
    }
} 