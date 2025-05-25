using UnityEngine;
using UnityEngine.AI;
using Zenject;
using System.Collections;

public class HittableEnemy : MonoBehaviour, IHittable, IEnemyComponent
{
    [SerializeField] private float hitForce = 10f;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private Vector3 centerOffset = Vector3.zero;
    [SerializeField] private float knockbackDuration = 1f;
    [SerializeField] private float deathAnimationDuration = 2f;
    [SerializeField] private float sinkDistance = 2f;
    [SerializeField] private float sinkDuration = 1f;
    [SerializeField] private float stunDuration = 2f;
    [SerializeField] private Animator _animator;
    [SerializeField] private UnityEngine.Animations.Rigging.Rig _aimRig;
    
    private IEnemyDetector _enemyDetector;
    private IEnemyHealthHandler _healthHandler;
    private IEnemyService _enemyService;
    private NavMeshAgent _navMeshAgent;
    private bool _isKnockedBack = false;
    private bool _isDead = false;
    private bool _isStunned = false;
    private Vector3 _originalPosition;
    private Enemy _enemy;
    private UnityEngine.Animations.Rigging.Rig _handsRig;
    private static readonly int Stunned = Animator.StringToHash("Stunned");
    
    public bool CanBeHit => !_isKnockedBack && !_isDead && !_isStunned && (_enemy?.Health?.IsAlive ?? false);
    public bool CanAct => !_isKnockedBack && !_isDead && !_isStunned && (_enemy?.Health?.IsAlive ?? false);
    public float Radius => radius;
    public Vector3 CenterOffset => centerOffset;
    public Enemy Enemy => _enemy;
    public float CollisionRadius => radius;
    public Vector3 CollisionCenterOffset => centerOffset;
    
    [Inject]
    public void Construct(IEnemyDetector enemyDetector, IEnemyHealthHandler healthHandler, IEnemyService enemyService)
    {
        _enemyDetector = enemyDetector;
        _healthHandler = healthHandler;
        _enemyService = enemyService;
    }
    
    public void SetEnemy(Enemy enemy)
    {
        _enemy = enemy;
        if (_enemy?.Health != null)
        {
            _enemy.Health.Died += OnEnemyDied;
        }
        OnRespawn();
    }
    
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _handsRig = GetComponentInChildren<UnityEngine.Animations.Rigging.Rig>();
    }
    
    // Вызывается из EnemyView при спавне
    public void OnRespawn()
    {
        _originalPosition = transform.position;
        _isDead = false;
        _isKnockedBack = false;
        _isStunned = false;
        
        if (_navMeshAgent != null)
            _navMeshAgent.enabled = true;
        if (_enemyDetector != null)
            _enemyDetector.RegisterEnemy(this);
        
        if (_handsRig != null)
            _handsRig.weight = 1f;
        if (_aimRig != null)
            _aimRig.weight = 1f;
        if (_animator != null)
            _animator.SetBool(Stunned, false);
    }
    
    // Вызывается из EnemyView при деспавне
    public void OnCleanup()
    {
        if (_enemyDetector != null)
            _enemyDetector.UnregisterEnemy(this);
        
        if (_enemy?.Health != null)
        {
            _enemy.Health.Died -= OnEnemyDied;
        }
    }
    
    public void OnHit(Vector3 hitDirection, float speed)
    {
        if (_isKnockedBack) return;
        
        StartCoroutine(KnockbackCoroutine(hitDirection, speed));
        StartCoroutine(StunCoroutine());
    }
    
    private IEnumerator KnockbackCoroutine(Vector3 hitDirection, float speed)
    {
        _isKnockedBack = true;
        
        if (_navMeshAgent != null)
        {
            _navMeshAgent.enabled = false;
        }
        
        float knockbackDistance = hitForce * speed * 0.1f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + hitDirection * knockbackDistance;
        
        float elapsed = 0f;
        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / knockbackDuration;
            float curve = 1f - t;
            
            transform.position = Vector3.Lerp(startPos, targetPos, 1f - curve * curve);
            yield return null;
        }
        
        transform.position = targetPos;
        
        if (_navMeshAgent != null && !_isDead)
        {
            _navMeshAgent.enabled = true;
            _navMeshAgent.Warp(transform.position);
        }
        
        _isKnockedBack = false;
    }
    
    private IEnumerator StunCoroutine()
    {
        _isStunned = true;
        
        if (_handsRig != null)
        {
            _handsRig.weight = 0f;
        }
        
        if (_aimRig != null)
        {
            _aimRig.weight = 0f;
        }
        
        if (_animator != null)
        {
            _animator.SetBool(Stunned, true);
        }
        
        yield return new WaitForSeconds(stunDuration);
        
        if (_animator != null)
        {
            _animator.SetBool(Stunned, false);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        _isStunned = false;
        
        if (_handsRig != null && !_isDead)
        {
            _handsRig.weight = 1f;
        }
        
        if (_aimRig != null && !_isDead)
        {
            _aimRig.weight = 1f;
        }
    }
    
    public void TakeDamageFromCar(float carDamage)
    {
        if (_enemy != null && _healthHandler != null)
        {
            _healthHandler.TakeDamage(_enemy, carDamage);
        }
        else if (!_isDead)
        {
            StartCoroutine(DeathSequence());
        }
    }
    
    private void OnEnemyDied()
    {
        if (!_isDead)
        {
            StartCoroutine(DeathSequence());
        }
    }
    
    private IEnumerator DeathSequence()
    {
        _isDead = true;
        
        if (_navMeshAgent != null)
        {
            _navMeshAgent.enabled = false;
        }
        
        if (_handsRig != null)
        {
            _handsRig.weight = 0f;
        }
        
        if (_aimRig != null)
        {
            _aimRig.weight = 0f;
        }
        
        if (_animator != null)
        {
            _animator.SetTrigger("Death");
        }
        
        yield return new WaitForSeconds(deathAnimationDuration);
        
        yield return StartCoroutine(SinkUnderground());
        
        ReturnToPool();
    }
    
    private IEnumerator SinkUnderground()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos - Vector3.up * sinkDistance;
        
        float elapsed = 0f;
        while (elapsed < sinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / sinkDuration;
            
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
    }
    
    private void ReturnToPool()
    {
        if (_enemyDetector != null)
        {
            _enemyDetector.UnregisterEnemy(this);
        }
        
        if (_enemy?.Health != null)
        {
            _enemy.Health.Died -= OnEnemyDied;
        }
        
        _isDead = false;
        _isKnockedBack = false;
        
        if (_navMeshAgent != null)
        {
            _navMeshAgent.enabled = true;
        }
        
        if (_enemy != null && _healthHandler != null)
        {
            float maxHealth = _healthHandler.GetMaxHealth(_enemy);
            float currentHealth = _healthHandler.GetCurrentHealth(_enemy);
            _healthHandler.Heal(_enemy, maxHealth - currentHealth);
        }
        
        if (_enemyService != null && _enemy != null)
        {
            _enemyService.DespawnEnemy(_enemy);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + centerOffset, radius);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position + centerOffset, radius);
    }
} 