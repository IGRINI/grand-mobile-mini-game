using UnityEngine;

public class RectangleObstacle : IObstacle
{
    public Vector3 Position { get; private set; }
    public float Radius { get; private set; }
    public Vector3 Size { get; private set; }
    public Quaternion Rotation { get; private set; }

    public RectangleObstacle(Vector3 position, Vector3 size, Quaternion rotation)
    {
        Position = position;
        Size = size;
        Rotation = rotation;
        Radius = Mathf.Max(size.x, size.z) * 0.5f;
    }

    public bool IsCollidingWith(Vector3 position, float radius)
    {
        Vector3 localPosition = Quaternion.Inverse(Rotation) * (position - Position);
        
        Vector3 halfSize = Size * 0.5f;
        Vector3 closestPoint = new Vector3(
            Mathf.Clamp(localPosition.x, -halfSize.x, halfSize.x),
            localPosition.y,
            Mathf.Clamp(localPosition.z, -halfSize.z, halfSize.z)
        );
        
        Vector3 difference = localPosition - closestPoint;
        difference.y = 0;
        
        return difference.sqrMagnitude <= radius * radius;
    }

    public bool IsCollidingWithRectangle(Vector3 otherPosition, Vector3 otherSize, Quaternion otherRotation)
    {
        return CheckRectangleOverlap(
            Position, Size, Rotation,
            otherPosition, otherSize, otherRotation
        );
    }

    private bool CheckRectangleOverlap(Vector3 pos1, Vector3 size1, Quaternion rot1, 
                                      Vector3 pos2, Vector3 size2, Quaternion rot2)
    {
        Vector3[] axes = new Vector3[4];
        axes[0] = rot1 * Vector3.right;
        axes[1] = rot1 * Vector3.forward;
        axes[2] = rot2 * Vector3.right;
        axes[3] = rot2 * Vector3.forward;

        for (int i = 0; i < 4; i++)
        {
            Vector3 axis = axes[i];
            axis.y = 0;
            axis = axis.normalized;

            if (!ProjectionsOverlap(axis, pos1, size1, rot1, pos2, size2, rot2))
            {
                return false;
            }
        }

        return true;
    }

    private bool ProjectionsOverlap(Vector3 axis, Vector3 pos1, Vector3 size1, Quaternion rot1,
                                   Vector3 pos2, Vector3 size2, Quaternion rot2)
    {
        float min1, max1, min2, max2;
        ProjectRectangle(axis, pos1, size1, rot1, out min1, out max1);
        ProjectRectangle(axis, pos2, size2, rot2, out min2, out max2);

        return !(max1 < min2 || max2 < min1);
    }

    private void ProjectRectangle(Vector3 axis, Vector3 position, Vector3 size, Quaternion rotation,
                                 out float min, out float max)
    {
        Vector3 halfSize = size * 0.5f;
        Vector3[] corners = new Vector3[4];
        corners[0] = position + rotation * new Vector3(-halfSize.x, 0, -halfSize.z);
        corners[1] = position + rotation * new Vector3(halfSize.x, 0, -halfSize.z);
        corners[2] = position + rotation * new Vector3(halfSize.x, 0, halfSize.z);
        corners[3] = position + rotation * new Vector3(-halfSize.x, 0, halfSize.z);

        min = max = Vector3.Dot(corners[0], axis);
        for (int i = 1; i < 4; i++)
        {
            float projection = Vector3.Dot(corners[i], axis);
            if (projection < min) min = projection;
            if (projection > max) max = projection;
        }
    }
} 