using UnityEngine;
using Zenject;

public class CameraOcclusionController : MonoBehaviour, ITickable
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform carTarget;
    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private bool debugMode = false;
    
    private IBuildingOcclusionDetector _occlusionDetector;
    private BuildingOcclusionDetector _concreteDetector;
    private float _lastCheckTime;

    [Inject]
    public void Construct(IBuildingOcclusionDetector occlusionDetector)
    {
        _occlusionDetector = occlusionDetector;
        _concreteDetector = occlusionDetector as BuildingOcclusionDetector;
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
        
        if (_concreteDetector != null)
            _concreteDetector.SetDebugMode(debugMode);
            
        if (debugMode)
        {
            Debug.Log($"[CameraOcclusionController] Initialized with camera: {targetCamera?.name}, target: {carTarget?.name}");
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
        {
            if (debugMode)
                Debug.LogWarning("[CameraOcclusionController] Missing components for occlusion check");
            return;
        }

        Vector3 cameraPosition = targetCamera.transform.position;
        Vector3 targetPosition = carTarget.position;

        _occlusionDetector.CheckOcclusion(cameraPosition, targetPosition);
    }

    public void SetCarTarget(Transform newTarget)
    {
        carTarget = newTarget;
        if (debugMode)
            Debug.Log($"[CameraOcclusionController] Car target changed to: {newTarget?.name}");
    }

    public void SetCheckInterval(float interval)
    {
        checkInterval = Mathf.Max(0.05f, interval);
        if (debugMode)
            Debug.Log($"[CameraOcclusionController] Check interval changed to: {checkInterval}");
    }
    
    public void SetDebugMode(bool enabled)
    {
        debugMode = enabled;
        if (_concreteDetector != null)
            _concreteDetector.SetDebugMode(enabled);
    }

    private void OnDrawGizmos()
    {
        if (!debugMode || targetCamera == null || carTarget == null) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(targetCamera.transform.position, carTarget.position);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetCamera.transform.position, 0.5f);
        Gizmos.DrawWireSphere(carTarget.position, 0.5f);
    }
} 