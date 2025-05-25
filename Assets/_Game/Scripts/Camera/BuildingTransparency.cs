using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zenject;

public class BuildingTransparency : MonoBehaviour, IBuildingTransparency
{
    [SerializeField] private float transparentAlpha = 0.3f;
    [SerializeField] private float animationDuration = 0.3f;
    
    private Renderer[] _renderers;
    private Material[] _originalMaterials;
    private Material[] _transparentMaterials;
    private Bounds _bounds;
    private bool _isTransparent = false;
    private Coroutine _animationCoroutine;
    private IBuildingOcclusionDetector _occlusionDetector;

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
        CalculateBounds();
    }

    private void Start()
    {
        _occlusionDetector?.RegisterBuilding(this);
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

    public void SetTransparency(float alpha, float duration = 0.3f)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine = StartCoroutine(AnimateToTransparency(alpha, duration));
    }

    public void RestoreOpacity(float duration = 0.3f)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine = StartCoroutine(AnimateToOpacity(duration));
    }

    public bool IsOccluding(Vector3 cameraPos, Vector3 targetPos)
    {
        return _bounds.IntersectRay(new Ray(cameraPos, (targetPos - cameraPos).normalized));
    }

    private IEnumerator AnimateToTransparency(float targetAlpha, float duration)
    {
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
        _animationCoroutine = null;
    }

    private IEnumerator AnimateToOpacity(float duration)
    {
        if (!_isTransparent) yield break;

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
        _animationCoroutine = null;
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