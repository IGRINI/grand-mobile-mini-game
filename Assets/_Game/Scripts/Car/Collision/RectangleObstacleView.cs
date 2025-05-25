using UnityEngine;
using Zenject;

public class RectangleObstacleView : MonoBehaviour
{
    [SerializeField] private Vector3 size = new Vector3(2f, 1f, 2f);
    [SerializeField] private Vector3 centerOffset = Vector3.zero;
    
    private IObstacle _obstacle;
    private IObstacleService _obstacleService;
    
    public IObstacle Obstacle => _obstacle ??= new RectangleObstacle(transform.position + centerOffset, size, transform.rotation);
    
    [Inject]
    public void Construct(IObstacleService obstacleService)
    {
        _obstacleService = obstacleService;
    }
    
    protected virtual void Awake()
    {
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
        Gizmos.matrix = Matrix4x4.TRS(transform.position + centerOffset, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = Matrix4x4.identity;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position + centerOffset, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, size);
        Gizmos.matrix = Matrix4x4.identity;
    }
}