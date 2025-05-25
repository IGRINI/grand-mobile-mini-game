using UnityEngine;

public class RandomTransform : MonoBehaviour
{
    [SerializeField] private Vector3 minScale = Vector3.one;
    [SerializeField] private Vector3 maxScale = Vector3.one;
    [SerializeField] private float minRotationY = 0f;
    [SerializeField] private float maxRotationY = 360f;

    private void Awake()
    {
        Vector3 randomScale = new Vector3(
            Random.Range(minScale.x, maxScale.x),
            Random.Range(minScale.y, maxScale.y),
            Random.Range(minScale.z, maxScale.z)
        );
        transform.localScale = randomScale;

        Vector3 euler = transform.localEulerAngles;
        euler.y = Random.Range(minRotationY, maxRotationY);
        transform.localEulerAngles = euler;
    }
} 