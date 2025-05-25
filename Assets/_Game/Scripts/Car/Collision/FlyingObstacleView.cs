using UnityEngine;
using System.Collections;
using Zenject;

public class FlyingObstacleView : ObstacleView, IHittable
{
    [SerializeField] private float hitForce = 10f;
    [SerializeField] private float upForce = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float destroyAfter = 5f;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(360f, 360f, 360f);

    private bool _isHit = false;
    private HittableCircleObstacle _hittableObstacle;

    public new IObstacle Obstacle => _hittableObstacle ??= new HittableCircleObstacle(transform.position + centerOffset, radius, this);

    public float CollisionRadius => radius;
    public Vector3 CollisionCenterOffset => centerOffset;

    public bool CanBeHit => !_isHit;
    public bool CanAct => !_isHit;

    private void Update()
    {
        if (_hittableObstacle != null)
        {
            _hittableObstacle.UpdatePosition(transform.position + centerOffset);
        }
    }

    protected override void Start()
    {
        Debug.Log($"FlyingObstacleView {name} starting, registering obstacle");
        if (_obstacleService != null)
        {
            _obstacleService.RegisterObstacle(Obstacle);
            Debug.Log($"FlyingObstacleView {name} registered obstacle successfully");
        }
        else
        {
            Debug.LogError($"FlyingObstacleView {name} - _obstacleService is null!");
        }
    }

    public void OnHit(Vector3 hitDirection, float speed)
    {
        Debug.Log($"FlyingObstacleView.OnHit called! Direction: {hitDirection}, Speed: {speed}, IsHit: {_isHit}");
        if (_isHit) return;
        _isHit = true;
        
        if (_obstacleService != null)
        {
            _obstacleService.UnregisterObstacle(Obstacle);
        }
        
        StartCoroutine(FlyAndDestroy(hitDirection, speed));
    }

    private IEnumerator FlyAndDestroy(Vector3 hitDirection, float speed)
    {
        Vector3 velocity = hitDirection.normalized * hitForce * speed * 0.1f + Vector3.up * upForce;
        float elapsed = 0f;
        while (elapsed < destroyAfter)
        {
            elapsed += Time.deltaTime;
            velocity += Vector3.up * gravity * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
            transform.Rotate(rotationSpeed * Time.deltaTime);
            if (transform.position.y < -10f) break;
            yield return null;
        }
        Destroy(gameObject);
    }
} 