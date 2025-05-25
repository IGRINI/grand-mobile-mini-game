using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zenject;

public class BuildingTransparency : MonoBehaviour, IBuildingTransparency
{
    [SerializeField] private float transparentAlpha = 0.3f;
    public float TransparentAlpha => transparentAlpha;
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private bool debugMode = false;
    [SerializeField] private bool occlusionEnabled = true;
    public bool OcclusionEnabled => occlusionEnabled;
    
    private Renderer[] _renderers;
    private Material[] _originalMaterials;
    private Material[] _transparentMaterials;
    private Bounds _bounds;
    private bool _isTransparent = false;
    private bool _isAnimating = false;
    private Coroutine _animationCoroutine;
    private IBuildingOcclusionDetector _occlusionDetector;
    private Bounds _localBounds;

    public Transform Transform => transform;
    public Bounds Bounds => _bounds;

    [Inject]
    public void Construct(IBuildingOcclusionDetector occlusionDetector)
    {
        _occlusionDetector = occlusionDetector;
    }

    private void Awake()
    {
        InitializeMaterials();
        CalculateLocalBounds();
        CalculateBounds();
    }

    private void Start()
    {
        _occlusionDetector?.RegisterBuilding(this);
        if (debugMode)
            Debug.Log($"[BuildingTransparency] Registered building: {gameObject.name}");
    }

    private void OnDestroy()
    {
        _occlusionDetector?.UnregisterBuilding(this);
        CleanupMaterials();
    }

    private void InitializeMaterials()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        var materialsList = new List<Material>();
        var transparentMaterialsList = new List<Material>();

        var transparentShader = Shader.Find("Custom/URP/WebGLMobileInstancedTransparentFade");
        if (transparentShader == null)
        {
            Debug.LogError("Transparent shader not found! Using fallback.");
            transparentShader = Shader.Find("Universal Render Pipeline/Lit");
        }

        foreach (var renderer in _renderers)
        {
            foreach (var material in renderer.materials)
            {
                materialsList.Add(material);
                
                var transparentMat = new Material(transparentShader);
                
                if (material.HasProperty("_BaseColor"))
                    transparentMat.SetColor("_BaseColor", material.GetColor("_BaseColor"));
                else if (material.HasProperty("_Color"))
                    transparentMat.SetColor("_BaseColor", material.GetColor("_Color"));
                    
                if (material.HasProperty("_MainTex"))
                    transparentMat.SetTexture("_MainTex", material.GetTexture("_MainTex"));
                    
                transparentMat.SetFloat("_Alpha", 1f);
                transparentMaterialsList.Add(transparentMat);
            }
        }

        _originalMaterials = materialsList.ToArray();
        _transparentMaterials = transparentMaterialsList.ToArray();
        
        if (debugMode)
            Debug.Log($"[BuildingTransparency] Initialized {_originalMaterials.Length} materials for {gameObject.name}");
    }

    private void CalculateBounds()
    {
        var bounds = new Bounds(transform.position, Vector3.zero);
        bool hasBounds = false;

        foreach (var renderer in _renderers)
        {
            if (hasBounds)
            {
                bounds.Encapsulate(renderer.bounds);
            }
            else
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
        }

        _bounds = bounds;
    }

    private void CalculateLocalBounds()
    {
        if (_renderers == null || _renderers.Length == 0)
        {
            _localBounds = new Bounds(Vector3.zero, Vector3.zero);
            return;
        }
        Bounds b = _renderers[0].localBounds;
        for (int i = 1; i < _renderers.Length; i++)
        {
            b.Encapsulate(_renderers[i].localBounds);
        }
        _localBounds = b;
    }

    public void SetTransparency(float alpha, float duration = 0.3f)
    {
        if (_isTransparent)
        {
            if (debugMode)
                Debug.Log($"[BuildingTransparency] Already transparent, skipping: {gameObject.name}");
            return;
        }

        if (debugMode)
            Debug.Log($"[BuildingTransparency] Setting transparency {alpha} for: {gameObject.name}");

        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine = StartCoroutine(AnimateToTransparency(alpha, duration));
    }

    public void RestoreOpacity(float duration = 0.3f)
    {
        if (!_isTransparent && !_isAnimating)
        {
            if (debugMode)
                Debug.Log($"[BuildingTransparency] Already opaque, skipping: {gameObject.name}");
            return;
        }

        if (debugMode)
            Debug.Log($"[BuildingTransparency] Restoring opacity for: {gameObject.name}");

        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine = StartCoroutine(AnimateToOpacity(duration));
    }

    public bool IsOccluding(Vector3 cameraPos, Vector3 targetPos)
    {
        Vector3 direction = (targetPos - cameraPos).normalized;
        float maxDistance = Vector3.Distance(cameraPos, targetPos);
        Ray worldRay = new Ray(cameraPos, direction);
        
        // Преобразуем луч в локальные координаты объекта
        Vector3 localOrigin = transform.InverseTransformPoint(worldRay.origin);
        Vector3 localDir = transform.InverseTransformDirection(worldRay.direction);
        localDir.Normalize();
        Ray localRay = new Ray(localOrigin, localDir);
        
        if (!_localBounds.IntersectRay(localRay, out float distance))
            return false;
        
        return distance <= maxDistance;
    }

    private IEnumerator AnimateToTransparency(float targetAlpha, float duration)
    {
        _isAnimating = true;
        
        if (!_isTransparent)
        {
            ApplyTransparentMaterials();
            _isTransparent = true;
        }

        float startAlpha = GetCurrentAlpha();
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            
            SetMaterialAlpha(currentAlpha);
            yield return null;
        }

        SetMaterialAlpha(targetAlpha);
        _isAnimating = false;
        _animationCoroutine = null;
        
        if (debugMode)
            Debug.Log($"[BuildingTransparency] Transparency animation completed for: {gameObject.name}");
    }

    private IEnumerator AnimateToOpacity(float duration)
    {
        if (!_isTransparent) 
        {
            _isAnimating = false;
            yield break;
        }

        _isAnimating = true;
        float startAlpha = GetCurrentAlpha();
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float currentAlpha = Mathf.Lerp(startAlpha, 1f, t);
            
            SetMaterialAlpha(currentAlpha);
            yield return null;
        }

        ApplyOriginalMaterials();
        _isTransparent = false;
        _isAnimating = false;
        _animationCoroutine = null;
        
        if (debugMode)
            Debug.Log($"[BuildingTransparency] Opacity animation completed for: {gameObject.name}");
    }

    private void ApplyTransparentMaterials()
    {
        int materialIndex = 0;
        foreach (var renderer in _renderers)
        {
            var materials = new Material[renderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = _transparentMaterials[materialIndex++];
            }
            renderer.materials = materials;
        }
        
        if (debugMode)
            Debug.Log($"[BuildingTransparency] Applied transparent materials to: {gameObject.name}");
    }

    private void ApplyOriginalMaterials()
    {
        int materialIndex = 0;
        foreach (var renderer in _renderers)
        {
            var materials = new Material[renderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = _originalMaterials[materialIndex++];
            }
            renderer.materials = materials;
        }
        
        if (debugMode)
            Debug.Log($"[BuildingTransparency] Applied original materials to: {gameObject.name}");
    }

    private void SetMaterialAlpha(float alpha)
    {
        if (!_isTransparent) return;

        foreach (var material in _transparentMaterials)
        {
            material.SetFloat("_Alpha", alpha);
        }
    }

    private float GetCurrentAlpha()
    {
        if (!_isTransparent || _transparentMaterials.Length == 0)
            return 1f;

        return _transparentMaterials[0].GetFloat("_Alpha");
    }

    private void CleanupMaterials()
    {
        if (_transparentMaterials != null)
        {
            foreach (var material in _transparentMaterials)
            {
                if (material != null)
                {
                    DestroyImmediate(material);
                }
            }
        }
    }
} 