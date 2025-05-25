using UnityEngine;

public interface IObstacle
{
    Vector3 Position { get; }
    float Radius { get; }
    bool IsCollidingWith(Vector3 position, float radius);
} 