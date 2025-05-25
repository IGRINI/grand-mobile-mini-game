using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class UpgradeCard : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image cardBackground;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button selectButton;
    [SerializeField] private GameObject levelIndicator;
    
    [Header("Animation Settings")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private Color hoverColor = Color.yellow;
    
    private UpgradeData _upgradeData;
    private int _currentLevel;
    private Color _originalColor;
    private Vector3 _originalScale;
    private bool _isHovered;
    
    public event Action<UpgradeData> OnUpgradeSelected;
    
    private void Awake()
    {
        if (selectButton != null)
            selectButton.onClick.AddListener(OnSelectClicked);
            
        if (cardBackground != null)
            _originalColor = cardBackground.color;
            
        _originalScale = transform.localScale;
        
        var eventTrigger = GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (eventTrigger == null)
            eventTrigger = gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            
        var pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => OnPointerEnter());
        eventTrigger.triggers.Add(pointerEnter);
        
        var pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => OnPointerExit());
        eventTrigger.triggers.Add(pointerExit);
    }
    
    private void OnDestroy()
    {
        if (selectButton != null)
            selectButton.onClick.RemoveListener(OnSelectClicked);
    }
    
    public void Setup(UpgradeData upgradeData, int currentLevel)
    {
        _upgradeData = upgradeData;
        _currentLevel = currentLevel;
        
        UpdateUI();
        AnimateAppearance();
    }
    
    private void UpdateUI()
    {
        if (_upgradeData == null) return;
        
        if (titleText != null)
            titleText.text = _upgradeData.upgradeName;
            
        if (descriptionText != null)
            descriptionText.text = _upgradeData.GetFormattedDescription(_currentLevel + 1);
            
        if (icon != null)
            icon.gameObject.SetActive(false);
            
        if (cardBackground != null)
            cardBackground.color = _upgradeData.cardColor;
            
        if (levelText != null)
        {
            if (_currentLevel > 0)
            {
                levelText.text = $"Уровень {_currentLevel} → {_currentLevel + 1}";
                levelText.gameObject.SetActive(true);
            }
            else
            {
                levelText.text = "НОВЫЙ";
                levelText.gameObject.SetActive(true);
            }
        }
        
        if (levelIndicator != null)
        {
            levelIndicator.SetActive(_currentLevel > 0);
        }
    }
    
    private void OnSelectClicked()
    {
        if (_upgradeData != null)
        {
            AnimateSelection();
            OnUpgradeSelected?.Invoke(_upgradeData);
        }
    }
    
    private void OnPointerEnter()
    {
        if (_isHovered) return;
        _isHovered = true;
        
        transform.DOScale(_originalScale * hoverScale, animationDuration)
            .SetEase(Ease.OutBack);
            
        if (cardBackground != null)
        {
            cardBackground.DOColor(hoverColor, animationDuration);
        }
    }
    
    private void OnPointerExit()
    {
        if (!_isHovered) return;
        _isHovered = false;
        
        transform.DOScale(_originalScale, animationDuration)
            .SetEase(Ease.OutBack);
            
        if (cardBackground != null)
        {
            cardBackground.DOColor(_originalColor, animationDuration);
        }
    }
    
    private void AnimateAppearance()
    {
        transform.localScale = Vector3.zero;
        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
        canvasGroup.alpha = 0f;
        
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(_originalScale, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(canvasGroup.DOFade(1f, 0.3f));
    }
    
    private void AnimateSelection()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(_originalScale * 1.1f, 0.1f));
        sequence.Append(transform.DOScale(_originalScale * 0.9f, 0.1f));
        sequence.Append(transform.DOScale(_originalScale, 0.1f));
        
        if (cardBackground != null)
        {
            cardBackground.DOColor(Color.green, 0.3f)
                .OnComplete(() => cardBackground.color = _originalColor);
        }
    }
} 