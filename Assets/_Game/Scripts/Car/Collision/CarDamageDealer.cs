using UnityEngine;
using Zenject;

public class CarDamageDealer : ICarDamageDealer
{
    private IUpgradeEffectProvider _upgradeEffectProvider;
    
    [Inject]
    public void Construct(IUpgradeEffectProvider upgradeEffectProvider)
    {
        _upgradeEffectProvider = upgradeEffectProvider;
    }
    
    public void DealDamage(IHittable target, float speed, float maxSpeed, float baseDamage)
    {
        float speedFactor = Mathf.Clamp01(Mathf.Abs(speed) / maxSpeed);
        float actualDamage = baseDamage * speedFactor;
        
        if (_upgradeEffectProvider != null)
        {
            float ramDamageMultiplier = _upgradeEffectProvider.GetRamDamageMultiplier();
            actualDamage *= ramDamageMultiplier;
        }
        
        if (target is HittableEnemy enemy)
        {
            enemy.TakeDamageFromCar(actualDamage);
        }
    }
} 