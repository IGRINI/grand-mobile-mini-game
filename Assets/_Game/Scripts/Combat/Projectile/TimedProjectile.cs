using UnityEngine;

public class TimedProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    
    private float _creationTime;

    public float Lifetime => lifetime;
    public float CreationTime => _creationTime;
    public bool IsExpired => Time.time - _creationTime >= lifetime;

    public void Initialize(float overrideLifetime = -1f)
    {
        _creationTime = Time.time;
        if (overrideLifetime > 0f)
            lifetime = overrideLifetime;
    }
} 