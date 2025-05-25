using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : ICollisionDetector
{
    private readonly List<IObstacle> _obstacles = new List<IObstacle>();

    public void AddObstacle(IObstacle obstacle)
    {
        _obstacles.Add(obstacle);
    }

    public void RemoveObstacle(IObstacle obstacle)
    {
        _obstacles.Remove(obstacle);
    }

    public bool CheckCollision(Vector3 currentPosition, Vector3 targetPosition, float carRadius)
    {
        foreach (var obstacle in _obstacles)
        {
            if (obstacle.IsCollidingWith(targetPosition, carRadius))
            {
                return true;
            }
        }
        return false;
    }
    
    public IObstacle GetCollidingObstacle(Vector3 position, float carRadius)
    {
        foreach (var obstacle in _obstacles)
        {
            if (obstacle.IsCollidingWith(position, carRadius))
            {
                return obstacle;
            }
        }
        return null;
    }

    public bool CheckRectangleCollision(Vector3 currentPosition, Vector3 targetPosition, Vector3 carSize, Quaternion carRotation)
    {
        foreach (var obstacle in _obstacles)
        {
            if (obstacle is RectangleObstacle rectObstacle)
            {
                if (rectObstacle.IsCollidingWithRectangle(targetPosition, carSize, carRotation))
                {
                    return true;
                }
            }
            else if (obstacle.IsCollidingWith(targetPosition, Mathf.Max(carSize.x, carSize.z) * 0.5f))
            {
                return true;
            }
        }
        return false;
    }

    public Vector3 GetCollisionNormal(Vector3 position, float carRadius)
    {
        foreach (var obstacle in _obstacles)
        {
            if (obstacle.IsCollidingWith(position, carRadius))
            {
                Vector3 direction = position - obstacle.Position;
                direction.y = 0;
                return direction.normalized;
            }
        }
        return Vector3.zero;
    }

    public Vector3 GetRectangleCollisionNormal(Vector3 position, Vector3 carSize, Quaternion carRotation)
    {
        foreach (var obstacle in _obstacles)
        {
            if (obstacle is RectangleObstacle rectObstacle)
            {
                if (rectObstacle.IsCollidingWithRectangle(position, carSize, carRotation))
                {
                    Vector3 direction = position - obstacle.Position;
                    direction.y = 0;
                    return direction.normalized;
                }
            }
            else if (obstacle.IsCollidingWith(position, Mathf.Max(carSize.x, carSize.z) * 0.5f))
            {
                Vector3 direction = position - obstacle.Position;
                direction.y = 0;
                return direction.normalized;
            }
        }
        return Vector3.zero;
    }

    public IObstacle GetCollidingObstacleRectangle(Vector3 position, Vector3 carSize, Quaternion carRotation)
    {
        foreach (var obstacle in _obstacles)
        {
            if (obstacle is RectangleObstacle rectObstacle)
            {
                if (rectObstacle.IsCollidingWithRectangle(position, carSize, carRotation))
                {
                    return obstacle;
                }
            }
            else if (obstacle.IsCollidingWith(position, Mathf.Max(carSize.x, carSize.z) * 0.5f))
            {
                return obstacle;
            }
        }
        return null;
    }

    public IHittable GetCollidingHittableObstacle(Vector3 position, Vector3 carSize, Quaternion carRotation)
    {
        foreach (var obstacle in _obstacles)
        {
            if (obstacle is IHittable hittable && hittable.CanBeHit)
            {
                MonoBehaviour mb = null;
                if (obstacle is HittableCircleObstacle hittableCircle)
                {
                    mb = hittableCircle._hittableComponent as MonoBehaviour;
                }
                else
                {
                    mb = hittable as MonoBehaviour;
                }
                
                if (mb == null || !mb.gameObject.activeInHierarchy) continue;

                if (obstacle is RectangleObstacle rectObstacle)
                {
                    if (rectObstacle.IsCollidingWithRectangle(position, carSize, carRotation))
                    {
                        Debug.Log($"Found colliding hittable rectangle obstacle: {mb.name}");
                        return hittable;
                    }
                }
                else if (obstacle.IsCollidingWith(position, Mathf.Max(carSize.x, carSize.z) * 0.5f))
                {
                    Debug.Log($"Found colliding hittable circle obstacle: {mb.name}");
                    return hittable;
                }
            }
        }
        return null;
    }

    public Vector3 GetPushOutDirection(Vector3 position, Vector3 carSize, Quaternion carRotation)
    {
        Vector3 totalPushDirection = Vector3.zero;
        int collisionCount = 0;

        foreach (var obstacle in _obstacles)
        {
            bool isColliding = false;
            Vector3 pushDirection = Vector3.zero;

            if (obstacle is RectangleObstacle rectObstacle)
            {
                if (rectObstacle.IsCollidingWithRectangle(position, carSize, carRotation))
                {
                    isColliding = true;
                    pushDirection = position - obstacle.Position;
                }
            }
            else if (obstacle.IsCollidingWith(position, Mathf.Max(carSize.x, carSize.z) * 0.5f))
            {
                isColliding = true;
                pushDirection = position - obstacle.Position;
            }

            if (isColliding)
            {
                pushDirection.y = 0;
                if (pushDirection.sqrMagnitude > 0.001f)
                {
                    totalPushDirection += pushDirection.normalized;
                    collisionCount++;
                }
            }
        }

        if (collisionCount > 0)
        {
            return (totalPushDirection / collisionCount).normalized;
        }

        return Vector3.zero;
    }
} 