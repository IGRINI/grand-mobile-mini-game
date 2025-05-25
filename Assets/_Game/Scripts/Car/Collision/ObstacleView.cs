using UnityEngine;
using Zenject;

public class ObstacleView : MonoBehaviour
{
    [SerializeField] private float radius = 1f;
    [SerializeField] private Vector3 centerOffset = Vector3.zero;
    
    private IObstacle _obstacle;
    private IObstacleService _obstacleService;
    
    public IObstacle Obstacle => _obstacle ??= new CircleObstacle(transform.position + centerOffset, radius);
    
    [Inject]
    public void Construct(IObstacleService obstacleService)
    {
        _obstacleService = obstacleService;
    }
    
    private void Start()
    {
        if (_obstacleService != null)
        {
            _obstacleService.RegisterObstacle(Obstacle);
        }
    }
    
    private void OnDestroy()
    {
        if (_obstacleService != null)
        {
            _obstacleService.UnregisterObstacle(Obstacle);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + centerOffset, radius);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position + centerOffset, radius);
    }
} 