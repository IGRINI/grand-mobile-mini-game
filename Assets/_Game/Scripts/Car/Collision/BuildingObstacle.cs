using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BuildingTransparency))]
public class BuildingObstacle : RectangleObstacleView, IDamageable
{
    [SerializeField] private bool canBeDestroyed = false;
    [SerializeField] private float health = 100f;
    [SerializeField] private float destructionDuration = 2f;
    [SerializeField] private float sinkDepth = 5f;
    [SerializeField] private float maxTiltAngle = 15f;
    [SerializeField] private float rotationSpeed = 90f;
    
    private BuildingTransparency _transparency;
    private bool _isDestroying = false;
    
    public bool CanBeDestroyed => canBeDestroyed;
    public float Health => health;
    public bool CanTakeDamage => canBeDestroyed && !_isDestroying;
    
    protected void Awake()
    {
        _transparency = GetComponent<BuildingTransparency>();
        if (_transparency == null)
        {
            _transparency = gameObject.AddComponent<BuildingTransparency>();
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (!canBeDestroyed || _isDestroying) return;
        
        health -= damage;
        if (health <= 0f)
        {
            DestroyBuilding();
        }
    }
    
    private void DestroyBuilding()
    {
        if (_isDestroying) return;
        _isDestroying = true;
        
        if (_obstacleService != null)
        {
            _obstacleService.UnregisterObstacle(Obstacle);
        }
        
        StartCoroutine(DestructionAnimation());
    }
    
    private IEnumerator DestructionAnimation()
    {
        Vector3 startPosition = transform.position;
        Vector3 startRotation = transform.eulerAngles;
        
        Vector3 targetPosition = startPosition + Vector3.down * sinkDepth;
        
        Vector3 randomTilt = new Vector3(
            Random.Range(-maxTiltAngle, maxTiltAngle),
            Random.Range(-maxTiltAngle, maxTiltAngle),
            Random.Range(-maxTiltAngle, maxTiltAngle)
        );
        Vector3 targetRotation = startRotation + randomTilt;
        
        float randomRotationDirection = Random.Range(-1f, 1f);
        float totalRotation = 0f;
        
        float elapsed = 0f;
        
        while (elapsed < destructionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / destructionDuration;
            
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);
            
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            transform.position = currentPosition;
            
            Vector3 currentRotation = Vector3.Lerp(startRotation, targetRotation, easedProgress);
            
            totalRotation += rotationSpeed * randomRotationDirection * Time.deltaTime;
            currentRotation.y += totalRotation;
            
            transform.eulerAngles = currentRotation;
            
            yield return null;
        }
        
        transform.position = targetPosition;
        transform.eulerAngles = targetRotation;
        
        yield return new WaitForSeconds(0.5f);
        
        Destroy(gameObject);
    }
} 