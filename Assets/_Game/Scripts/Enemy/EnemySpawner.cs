using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Vector3 spawnBoundsMin = new Vector3(-10f, 0f, -10f);
    [SerializeField] private Vector3 spawnBoundsMax = new Vector3(10f, 0f, 10f);
    [SerializeField] private float minSpawnDistance = 0f;
    [SerializeField] private float maxSpawnDistance = 20f;
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
        Vector3 playerPos = _carView.Transform.position;
        float sqrMin = minSpawnDistance * minSpawnDistance;
        float sqrMax = maxSpawnDistance * maxSpawnDistance;
        
        for (int i = 0; i < 50; i++)
        {
            float x = Random.Range(spawnBoundsMin.x, spawnBoundsMax.x);
            float y = spawnBoundsMin.y;
            float z = Random.Range(spawnBoundsMin.z, spawnBoundsMax.z);
            Vector3 localPos = new Vector3(x, y, z);
            Vector3 worldPos = transform.TransformPoint(localPos);
            
            if (NavMesh.SamplePosition(worldPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                Vector3 navMeshPos = hit.position;
                float sqrDist = (navMeshPos - playerPos).sqrMagnitude;
                if (sqrDist >= sqrMin && sqrDist <= sqrMax)
                {
                    Debug.Log($"Spawning enemy at NavMesh position: {navMeshPos}, distance from player: {Mathf.Sqrt(sqrDist):F2}");
                    return navMeshPos;
                }
            }
        }
        
        float fallbackX = Random.Range(spawnBoundsMin.x, spawnBoundsMax.x);
        float fallbackY = spawnBoundsMin.y;
        float fallbackZ = Random.Range(spawnBoundsMin.z, spawnBoundsMax.z);
        Vector3 fallbackLocalPos = new Vector3(fallbackX, fallbackY, fallbackZ);
        Vector3 fallbackWorldPos = transform.TransformPoint(fallbackLocalPos);
        
        if (NavMesh.SamplePosition(fallbackWorldPos, out NavMeshHit fallbackHit, 10f, NavMesh.AllAreas))
        {
            Debug.LogWarning($"Using fallback NavMesh position: {fallbackHit.position}");
            return fallbackHit.position;
        }
        
        Debug.LogError($"Could not find NavMesh position, using world position: {fallbackWorldPos}");
        return fallbackWorldPos;
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
        
        if (_carView?.Transform != null)
        {
            Vector3 playerPos = _carView.Transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerPos, maxSpawnDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerPos, minSpawnDistance);
        }
    }
} 