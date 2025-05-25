using UnityEngine;
using System.Collections;
using Zenject;

public class FlyingObstacleView : MonoBehaviour, IHittable
{
    [SerializeField] private float hitForce = 10f;
    [SerializeField] private float upForce = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float destroyAfter = 5f;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(360f, 360f, 360f);

    private IEnemyDetector _enemyDetector;
    private bool _isHit = false;

    [Inject]
    public void Construct(IEnemyDetector enemyDetector)
    {
        _enemyDetector = enemyDetector;
    }

    private void Start()
    {
        _enemyDetector?.RegisterEnemy(this);
    }

    private void OnDestroy()
    {
        _enemyDetector?.UnregisterEnemy(this);
    }

    public bool CanBeHit => !_isHit;
    public bool CanAct => !_isHit;

    public void OnHit(Vector3 hitDirection, float speed)
    {
        if (_isHit) return;
        _isHit = true;
        _enemyDetector?.UnregisterEnemy(this);
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