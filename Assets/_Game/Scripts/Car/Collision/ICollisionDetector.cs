using UnityEngine;

public interface ICollisionDetector
{
    bool CheckCollision(Vector3 currentPosition, Vector3 targetPosition, float carRadius);
    bool CheckRectangleCollision(Vector3 currentPosition, Vector3 targetPosition, Vector3 carSize, Quaternion carRotation);
    Vector3 GetCollisionNormal(Vector3 position, float carRadius);
    Vector3 GetRectangleCollisionNormal(Vector3 position, Vector3 carSize, Quaternion carRotation);
    IObstacle GetCollidingObstacle(Vector3 position, float carRadius);
    IObstacle GetCollidingObstacleRectangle(Vector3 position, Vector3 carSize, Quaternion carRotation);
    IHittable GetCollidingHittableObstacle(Vector3 position, Vector3 carSize, Quaternion carRotation);
    Vector3 GetPushOutDirection(Vector3 position, Vector3 carSize, Quaternion carRotation);
} 