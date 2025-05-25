using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private float restartDelay = 2f;
    
    private IHealthService _healthService;
    private CarController _carController;
    private bool _gameOver = false;
    
    [Inject]
    public void Construct(IHealthService healthService, CarController carController)
    {
        _healthService = healthService;
        _carController = carController;
    }
    
    private void Start()
    {
        if (_healthService != null && _carController != null)
        {
            _healthService.EntityDied += OnEntityDied;
        }
    }
    
    private void OnDestroy()
    {
        if (_healthService != null)
        {
            _healthService.EntityDied -= OnEntityDied;
        }
    }
    
    private void OnEntityDied(object entity, IHealth health)
    {
        if (entity == _carController && !_gameOver)
        {
            _gameOver = true;
            Debug.Log("Машина уничтожена! Перезагружаем сцену...");
            StartCoroutine(RestartSceneAfterDelay());
        }
    }
    
    private IEnumerator RestartSceneAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Перезагружаем сцену: {currentSceneName}");
        SceneManager.LoadScene(currentSceneName);
    }
} 