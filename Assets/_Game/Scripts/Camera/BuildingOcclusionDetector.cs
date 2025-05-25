using UnityEngine;
using System.Collections.Generic;

public class BuildingOcclusionDetector : IBuildingOcclusionDetector
{
    private readonly List<IBuildingTransparency> _buildings = new List<IBuildingTransparency>();
    private readonly HashSet<IBuildingTransparency> _occludedBuildings = new HashSet<IBuildingTransparency>();
    
    private const float OCCLUSION_THRESHOLD = 0.1f;

    public void RegisterBuilding(IBuildingTransparency building)
    {
        if (!_buildings.Contains(building))
        {
            _buildings.Add(building);
        }
    }

    public void UnregisterBuilding(IBuildingTransparency building)
    {
        _buildings.Remove(building);
        _occludedBuildings.Remove(building);
    }

    public void CheckOcclusion(Vector3 cameraPosition, Vector3 targetPosition)
    {
        var currentlyOccluded = new HashSet<IBuildingTransparency>();
        
        Vector3 rayDirection = (targetPosition - cameraPosition).normalized;
        float rayDistance = Vector3.Distance(cameraPosition, targetPosition);
        
        foreach (var building in _buildings)
        {
            if (IsRayIntersectingBounds(cameraPosition, rayDirection, rayDistance, building.Bounds))
            {
                currentlyOccluded.Add(building);
                
                if (!_occludedBuildings.Contains(building))
                {
                    building.SetTransparency(0.3f);
                }
            }
        }
        
        foreach (var building in _occludedBuildings)
        {
            if (!currentlyOccluded.Contains(building))
            {
                building.RestoreOpacity();
            }
        }
        
        _occludedBuildings.Clear();
        foreach (var building in currentlyOccluded)
        {
            _occludedBuildings.Add(building);
        }
    }

    private bool IsRayIntersectingBounds(Vector3 rayOrigin, Vector3 rayDirection, float rayDistance, Bounds bounds)
    {
        Vector3 invDir = new Vector3(
            Mathf.Approximately(rayDirection.x, 0) ? float.MaxValue : 1f / rayDirection.x,
            Mathf.Approximately(rayDirection.y, 0) ? float.MaxValue : 1f / rayDirection.y,
            Mathf.Approximately(rayDirection.z, 0) ? float.MaxValue : 1f / rayDirection.z
        );

        Vector3 t1 = Vector3.Scale(bounds.min - rayOrigin, invDir);
        Vector3 t2 = Vector3.Scale(bounds.max - rayOrigin, invDir);

        Vector3 tMin = Vector3.Min(t1, t2);
        Vector3 tMax = Vector3.Max(t1, t2);

        float tNear = Mathf.Max(Mathf.Max(tMin.x, tMin.y), tMin.z);
        float tFar = Mathf.Min(Mathf.Min(tMax.x, tMax.y), tMax.z);

        if (tNear > tFar || tFar < 0 || tNear > rayDistance)
            return false;

        return tNear >= OCCLUSION_THRESHOLD;
    }
} 