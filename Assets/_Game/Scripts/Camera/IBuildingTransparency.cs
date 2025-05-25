using UnityEngine;

public interface IBuildingTransparency
{
    Transform Transform { get; }
    Bounds Bounds { get; }
    void SetTransparency(float alpha, float duration = 0.3f);
    void RestoreOpacity(float duration = 0.3f);
    bool IsOccluding(Vector3 cameraPos, Vector3 targetPos);
    float TransparentAlpha { get; }
    bool OcclusionEnabled { get; }
} 