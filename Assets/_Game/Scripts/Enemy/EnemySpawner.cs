using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnRadius = 10f;
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
        var playerPosition = _carView.Transform.position;
        var randomDirection = Random.insideUnitCircle.normalized;
        var spawnOffset = new Vector3(randomDirection.x, 0f, randomDirection.y) * spawnRadius;
        
        return playerPosition + spawnOffset;
    }
} 