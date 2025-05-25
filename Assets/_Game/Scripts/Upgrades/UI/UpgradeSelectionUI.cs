using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using DG.Tweening;
using System.Collections.Generic;

public class UpgradeSelectionUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private UpgradeCard cardPrefab;
    [SerializeField] private Button skipButton;
    [SerializeField] private Image backgroundOverlay;
    
    [Header("Animation Settings")]
    [SerializeField] private float panelAnimationDuration = 0.5f;
    [SerializeField] private float cardSpacing = 50f;
    [SerializeField] private float cardAppearDelay = 0.1f;
    
    private IUpgradeService _upgradeService;
    private List<UpgradeCard> _currentCards = new();
    private bool _isSelectionActive;
    
    [Inject]
    public void Construct(IUpgradeService upgradeService)
    {
        _upgradeService = upgradeService;
    }
    
    private void Start()
    {
        if (_upgradeService != null)
        {
            _upgradeService.UpgradeOptionsAvailable += OnUpgradeOptionsAvailable;
            if (_upgradeService is UpgradeService service) service.UpgradeSelectionClosed += OnUpgradeSelectionClosed;
        }
        
        if (skipButton != null)
            skipButton.onClick.AddListener(OnSkipClicked);
            
        if (upgradePanel != null)
            upgradePanel.SetActive(false);
    }
    
    private void OnDestroy()
    {
        if (_upgradeService != null)
        {
            _upgradeService.UpgradeOptionsAvailable -= OnUpgradeOptionsAvailable;
            if (_upgradeService is UpgradeService service) service.UpgradeSelectionClosed -= OnUpgradeSelectionClosed;
        }
        
        if (skipButton != null)
            skipButton.onClick.RemoveListener(OnSkipClicked);
    }
    
    private void OnUpgradeOptionsAvailable(UpgradeData[] options)
    {
        ShowUpgradeSelection(options);
    }
    
    private void OnUpgradeSelectionClosed()
    {
        HideUpgradeSelection();
    }
    
    private void ShowUpgradeSelection(UpgradeData[] options)
    {
        if (_isSelectionActive) return;
        
        _isSelectionActive = true;
        Time.timeScale = 0f;
        
        if (upgradePanel != null)
            upgradePanel.SetActive(true);
            
        if (titleText != null)
            titleText.text = "ВЫБЕРИТЕ АПГРЕЙД";
            
        CreateUpgradeCards(options);
        AnimateShowPanel();
    }
    
    private void HideUpgradeSelection()
    {
        if (!_isSelectionActive) return;
        
        AnimateHidePanel(() =>
        {
            _isSelectionActive = false;
            Time.timeScale = 1f;
            
            if (upgradePanel != null)
                upgradePanel.SetActive(false);
                
            ClearCards();
        });
    }
    
    private void CreateUpgradeCards(UpgradeData[] options)
    {
        ClearCards();
        
        if (cardPrefab == null || cardsContainer == null) return;
        
        for (int i = 0; i < options.Length; i++)
        {
            var upgradeData = options[i];
            var currentLevel = _upgradeService.GetUpgradeLevel(upgradeData.upgradeType);
            
            var cardInstance = Instantiate(cardPrefab, cardsContainer);
            cardInstance.Setup(upgradeData, currentLevel);
            cardInstance.OnUpgradeSelected += OnUpgradeCardSelected;
            
            var delay = i * cardAppearDelay;
            cardInstance.transform.localScale = Vector3.zero;
            cardInstance.transform.DOScale(Vector3.one, 0.3f)
                .SetDelay(delay)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
            
            _currentCards.Add(cardInstance);
        }
    }
    
    private void ClearCards()
    {
        foreach (var card in _currentCards)
        {
            if (card != null)
            {
                card.OnUpgradeSelected -= OnUpgradeCardSelected;
                Destroy(card.gameObject);
            }
        }
        _currentCards.Clear();
    }
    
    private void OnUpgradeCardSelected(UpgradeData upgradeData)
    {
        foreach (var card in _currentCards)
        {
            if (card != null)
                card.OnUpgradeSelected -= OnUpgradeCardSelected;
        }
        
        _upgradeService.SelectUpgrade(upgradeData);
    }
    
    private void OnSkipClicked()
    {
        if (_upgradeService is UpgradeService service)
        {
            service.UpgradeSelectionClosed?.Invoke();
        }
    }
    
    private void AnimateShowPanel()
    {
        if (upgradePanel == null) return;
        
        var canvasGroup = upgradePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = upgradePanel.AddComponent<CanvasGroup>();
            
        canvasGroup.alpha = 0f;
        upgradePanel.transform.localScale = Vector3.zero;
        
        if (backgroundOverlay != null)
        {
            backgroundOverlay.color = new Color(0, 0, 0, 0);
            backgroundOverlay.DOFade(0.7f, panelAnimationDuration).SetUpdate(true);
        }
        
        var sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1f, panelAnimationDuration).SetUpdate(true));
        sequence.Join(upgradePanel.transform.DOScale(Vector3.one, panelAnimationDuration)
            .SetEase(Ease.OutBack).SetUpdate(true));
    }
    
    private void AnimateHidePanel(System.Action onComplete = null)
    {
        if (upgradePanel == null)
        {
            onComplete?.Invoke();
            return;
        }
        
        var canvasGroup = upgradePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = upgradePanel.AddComponent<CanvasGroup>();
        
        if (backgroundOverlay != null)
        {
            backgroundOverlay.DOFade(0f, panelAnimationDuration).SetUpdate(true);
        }
        
        var sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(0f, panelAnimationDuration).SetUpdate(true));
        sequence.Join(upgradePanel.transform.DOScale(Vector3.zero, panelAnimationDuration)
            .SetEase(Ease.InBack).SetUpdate(true));
        sequence.OnComplete(() => onComplete?.Invoke());
    }
} 