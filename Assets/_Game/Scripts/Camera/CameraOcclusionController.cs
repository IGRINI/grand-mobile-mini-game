using UnityEngine;
using Zenject;

public class CameraOcclusionController : MonoBehaviour, ITickable
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform carTarget;
    [SerializeField] private float checkInterval = 0.1f;
    
    private IBuildingOcclusionDetector _occlusionDetector;
    private float _lastCheckTime;

    [Inject]
    public void Construct(IBuildingOcclusionDetector occlusionDetector)
    {
        _occlusionDetector = occlusionDetector;
    }

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
            
        if (carTarget == null)
        {
            var carView = FindObjectOfType<CarView>();
            if (carView != null)
                carTarget = carView.Transform;
        }
    }

    public void Tick()
    {
        if (Time.time - _lastCheckTime >= checkInterval)
        {
            CheckOcclusion();
            _lastCheckTime = Time.time;
        }
    }

    private void CheckOcclusion()
    {
        if (targetCamera == null || carTarget == null || _occlusionDetector == null)
            return;

        Vector3 cameraPosition = targetCamera.transform.position;
        Vector3 targetPosition = carTarget.position;

        _occlusionDetector.CheckOcclusion(cameraPosition, targetPosition);
    }

    public void SetCarTarget(Transform newTarget)
    {
        carTarget = newTarget;
    }

    public void SetCheckInterval(float interval)
    {
        checkInterval = Mathf.Max(0.05f, interval);
    }
} 