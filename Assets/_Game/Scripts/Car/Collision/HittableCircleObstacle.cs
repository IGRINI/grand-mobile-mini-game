using UnityEngine;

public class HittableCircleObstacle : IObstacle, IHittable
{
    public readonly IHittable _hittableComponent;
    
    public Vector3 Position { get; private set; }
    public float Radius { get; private set; }
    
    public float CollisionRadius => Radius;
    public Vector3 CollisionCenterOffset => Vector3.zero;
    public bool CanBeHit => _hittableComponent.CanBeHit;
    public bool CanAct => _hittableComponent.CanAct;

    public HittableCircleObstacle(Vector3 position, float radius, IHittable hittableComponent)
    {
        Position = position;
        Radius = radius;
        _hittableComponent = hittableComponent;
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        Position = newPosition;
    }

    public bool IsCollidingWith(Vector3 position, float radius)
    {
        float sqrDistance = (Position - position).sqrMagnitude;
        float totalRadius = Radius + radius;
        return sqrDistance <= totalRadius * totalRadius;
    }

    public void OnHit(Vector3 hitDirection, float speed)
    {
        _hittableComponent.OnHit(hitDirection, speed);
    }
} 