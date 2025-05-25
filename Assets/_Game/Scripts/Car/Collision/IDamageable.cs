public interface IDamageable
{
    void TakeDamage(float damage);
    bool CanTakeDamage { get; }
} 