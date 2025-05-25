using UnityEngine;

public interface IHittable
{
    void OnHit(Vector3 hitDirection, float speed);
    bool CanBeHit { get; }
    bool CanAct { get; }
} 