using UnityEngine;

[RequireComponent(typeof(BuildingTransparency))]
public class BuildingObstacle : RectangleObstacleView, IDamageable
{
    [SerializeField] private bool canBeDestroyed = false;
    [SerializeField] private float health = 100f;
    
    private BuildingTransparency _transparency;
    
    public bool CanBeDestroyed => canBeDestroyed;
    public float Health => health;
    public bool CanTakeDamage => canBeDestroyed;
    
    protected override void Awake()
    {
        base.Awake();
        _transparency = GetComponent<BuildingTransparency>();
        if (_transparency == null)
        {
            _transparency = gameObject.AddComponent<BuildingTransparency>();
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (!canBeDestroyed) return;
        
        health -= damage;
        if (health <= 0f)
        {
            DestroyBuilding();
        }
    }
    
    private void DestroyBuilding()
    {
        Destroy(gameObject);
    }
} 