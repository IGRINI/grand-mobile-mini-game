using UnityEngine;
using System.Collections.Generic;

public class BuildingOcclusionDetector : IBuildingOcclusionDetector
{
    private readonly List<IBuildingTransparency> _buildings = new List<IBuildingTransparency>();
    private readonly HashSet<IBuildingTransparency> _occludedBuildings = new HashSet<IBuildingTransparency>();
    
    private const float OCCLUSION_THRESHOLD = 0.1f;
    private bool _debugMode = false;

    public void SetDebugMode(bool enabled)
    {
        _debugMode = enabled;
    }

    public void RegisterBuilding(IBuildingTransparency building)
    {
        if (!_buildings.Contains(building))
        {
            _buildings.Add(building);
            if (_debugMode)
                Debug.Log($"[OcclusionDetector] Registered building: {building.Transform.name}");
        }
    }

    public void UnregisterBuilding(IBuildingTransparency building)
    {
        _buildings.Remove(building);
        _occludedBuildings.Remove(building);
        if (_debugMode)
            Debug.Log($"[OcclusionDetector] Unregistered building: {building.Transform.name}");
    }

    public void CheckOcclusion(Vector3 cameraPosition, Vector3 targetPosition)
    {
        if (_debugMode)
            Debug.Log($"[OcclusionDetector] Checking occlusion from {cameraPosition} to {targetPosition}");

        var currentlyOccluded = new HashSet<IBuildingTransparency>();
        
        Vector3 rayDirection = (targetPosition - cameraPosition).normalized;
        float rayDistance = Vector3.Distance(cameraPosition, targetPosition);
        
        int checkedBuildings = 0;
        int occludedCount = 0;
        
        foreach (var building in _buildings)
        {
            if (!building.OcclusionEnabled) continue;
            if (building?.Transform == null) continue;
            
            checkedBuildings++;
            
            if (building.IsOccluding(cameraPosition, targetPosition))
            {
                currentlyOccluded.Add(building);
                occludedCount++;
                
                if (!_occludedBuildings.Contains(building))
                {
                    building.SetTransparency(building.TransparentAlpha);
                    if (_debugMode)
                        Debug.Log($"[OcclusionDetector] Making transparent: {building.Transform.name}");
                }
            }
        }
        
        int restoredCount = 0;
        foreach (var building in _occludedBuildings)
        {
            if (!currentlyOccluded.Contains(building))
            {
                building.RestoreOpacity();
                restoredCount++;
                if (_debugMode)
                    Debug.Log($"[OcclusionDetector] Restoring opacity: {building.Transform.name}");
            }
        }
        
        _occludedBuildings.Clear();
        foreach (var building in currentlyOccluded)
        {
            _occludedBuildings.Add(building);
        }
        
        if (_debugMode)
            Debug.Log($"[OcclusionDetector] Checked {checkedBuildings} buildings, {occludedCount} occluded, {restoredCount} restored");
    }
} 