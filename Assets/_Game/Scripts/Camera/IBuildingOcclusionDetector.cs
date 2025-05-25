using UnityEngine;
using System.Collections.Generic;

public interface IBuildingOcclusionDetector
{
    void RegisterBuilding(IBuildingTransparency building);
    void UnregisterBuilding(IBuildingTransparency building);
    void CheckOcclusion(Vector3 cameraPosition, Vector3 targetPosition);
} 