using UnityEngine;

public interface ICollisionEffect
{
    void ApplyCollisionEffect(Vector3 collisionNormal, float speed, float maxSpeed);
} 