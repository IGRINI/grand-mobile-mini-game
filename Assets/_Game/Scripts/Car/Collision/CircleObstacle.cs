using UnityEngine;

public class CircleObstacle : IObstacle
{
    public Vector3 Position { get; private set; }
    public float Radius { get; private set; }

    public CircleObstacle(Vector3 position, float radius)
    {
        Position = position;
        Radius = radius;
    }

    public bool IsCollidingWith(Vector3 position, float radius)
    {
        float sqrDistance = (Position - position).sqrMagnitude;
        float totalRadius = Radius + radius;
        return sqrDistance <= totalRadius * totalRadius;
    }
} 