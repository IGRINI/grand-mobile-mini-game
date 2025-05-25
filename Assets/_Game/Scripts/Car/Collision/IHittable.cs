using UnityEngine;

public interface IHittable
{
    void OnHit(Vector3 hitDirection, float speed);
    bool CanBeHit { get; }
    bool CanAct { get; }
    float CollisionRadius { get; }
    Vector3 CollisionCenterOffset { get; }
} 