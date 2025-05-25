using UnityEngine;

public interface ICarDamageDealer
{
    void DealDamage(IHittable target, float speed, float maxSpeed, float baseDamage);
} 