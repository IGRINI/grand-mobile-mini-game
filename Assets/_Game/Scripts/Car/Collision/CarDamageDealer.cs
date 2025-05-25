using UnityEngine;

public class CarDamageDealer : ICarDamageDealer
{
    public void DealDamage(IHittable target, float speed, float maxSpeed, float baseDamage)
    {
        float speedFactor = Mathf.Clamp01(Mathf.Abs(speed) / maxSpeed);
        float actualDamage = baseDamage * speedFactor;
        
        if (target is HittableEnemy enemy)
        {
            enemy.TakeDamageFromCar(actualDamage);
        }
    }
} 