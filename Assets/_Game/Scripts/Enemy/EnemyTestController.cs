using UnityEngine;
using Zenject;
using UnityEngine.InputSystem;

public class EnemyTestController : MonoBehaviour
{
    [SerializeField] private EnemyData testEnemyData;
    [SerializeField] private float spawnDistance = 5f;
    private IInputService _inputService;
    private IEnemyService _enemyService;
    private ICarView _carView;

    [Inject]
    public void Construct(IEnemyService enemyService, ICarView carView, IInputService inputService)
    {
        _enemyService = enemyService;
        _carView = carView;
        _inputService = inputService;
    }

    private void Update()
    {
        if (_inputService.Interact && testEnemyData != null)
            SpawnTestEnemy();
        if (_inputService.Crouch)
            ClearAllEnemies();
    }

    private void SpawnTestEnemy()
    {
        var playerPosition = _carView?.Transform.position ?? Vector3.zero;
        var randomDirection = Random.insideUnitCircle.normalized;
        var spawnPosition = playerPosition + new Vector3(randomDirection.x, 0f, randomDirection.y) * spawnDistance;
        
        var enemy = _enemyService.SpawnEnemy(testEnemyData, spawnPosition);
        Debug.Log($"Заспавнил врага: {enemy.Name} на позиции {spawnPosition}");
    }

    private void ClearAllEnemies()
    {
        var enemies = _enemyService.ActiveEnemies;
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            _enemyService.DespawnEnemy(enemies[i]);
        }
        Debug.Log("Убрал всех врагов");
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 100));
        GUILayout.Label($"Врагов: {_enemyService?.ActiveEnemies.Count ?? 0}");
        GUILayout.Label($"Нажми E - создать врага");
        GUILayout.Label("Нажми C - убрать всех");
        GUILayout.EndArea();
    }
} 