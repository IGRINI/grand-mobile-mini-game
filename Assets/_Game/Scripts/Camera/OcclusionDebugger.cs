using UnityEngine;

public class OcclusionDebugger : MonoBehaviour
{
    [SerializeField] private KeyCode toggleDebugKey = KeyCode.F1;
    [SerializeField] private KeyCode testTransparencyKey = KeyCode.F2;
    
    private CameraOcclusionController _occlusionController;
    private BuildingTransparency[] _buildingTransparencies;
    private bool _debugEnabled = false;

    private void Start()
    {
        _occlusionController = FindObjectOfType<CameraOcclusionController>();
        _buildingTransparencies = FindObjectsOfType<BuildingTransparency>();
        
        Debug.Log($"[OcclusionDebugger] Found {_buildingTransparencies.Length} buildings with transparency");
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleDebugKey))
        {
            ToggleDebug();
        }
        
        if (Input.GetKeyDown(testTransparencyKey))
        {
            TestTransparency();
        }
    }

    private void ToggleDebug()
    {
        _debugEnabled = !_debugEnabled;
        
        if (_occlusionController != null)
        {
            _occlusionController.SetDebugMode(_debugEnabled);
        }
        
        Debug.Log($"[OcclusionDebugger] Debug mode: {(_debugEnabled ? "ENABLED" : "DISABLED")}");
        Debug.Log($"Press {toggleDebugKey} to toggle debug, {testTransparencyKey} to test transparency");
    }

    private void TestTransparency()
    {
        if (_buildingTransparencies.Length == 0)
        {
            Debug.LogWarning("[OcclusionDebugger] No buildings found for transparency test");
            return;
        }

        var randomBuilding = _buildingTransparencies[Random.Range(0, _buildingTransparencies.Length)];
        randomBuilding.SetTransparency(0.3f);
        
        Debug.Log($"[OcclusionDebugger] Testing transparency on: {randomBuilding.name}");
        
        // Restore after 3 seconds
        Invoke(nameof(RestoreTestBuilding), 3f);
    }

    private void RestoreTestBuilding()
    {
        foreach (var building in _buildingTransparencies)
        {
            building.RestoreOpacity();
        }
        Debug.Log("[OcclusionDebugger] Restored all buildings opacity");
    }

    private void OnGUI()
    {
        if (!_debugEnabled) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Occlusion Debug Info:");
        GUILayout.Label($"Buildings found: {_buildingTransparencies.Length}");
        GUILayout.Label($"Controller: {(_occlusionController != null ? "OK" : "MISSING")}");
        GUILayout.Label($"Press {toggleDebugKey} - Toggle Debug");
        GUILayout.Label($"Press {testTransparencyKey} - Test Transparency");
        GUILayout.EndArea();
    }
} 