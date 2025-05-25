using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using DG.Tweening;

public class ExperienceUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private Image experienceBar;
    [SerializeField] private GameObject levelUpNotification;
    [SerializeField] private TextMeshProUGUI levelUpText;
    [SerializeField] private GameObject experienceGainNotification;
    [SerializeField] private TextMeshProUGUI experienceGainText;
    
    [Header("Animation Settings")]
    [SerializeField] private float barAnimationDuration = 0.5f;
    [SerializeField] private float notificationDuration = 2f;
    [SerializeField] private Color levelUpColor = Color.yellow;
    [SerializeField] private Color experienceGainColor = Color.green;
    
    private IExperienceService _experienceService;
    private Tween _barTween;
    private Tween _punch; 

    [Inject]
    public void Construct(IExperienceService experienceService)
    {
        _experienceService = experienceService;
    }
    
    private void Start()
    {
        if (_experienceService != null)
        {
            _experienceService.LevelChanged += OnLevelChanged;
            _experienceService.ExperienceChanged += OnExperienceChanged;
            _experienceService.LevelUp += OnLevelUp;
            _experienceService.ExperienceGained += OnExperienceGained;
            
            UpdateUI();
        }
        
        if (levelUpNotification != null)
            levelUpNotification.SetActive(false);
            
        if (experienceGainNotification != null)
            experienceGainNotification.SetActive(false);
    }
    
    private void OnDestroy()
    {
        if (_experienceService != null)
        {
            _experienceService.LevelChanged -= OnLevelChanged;
            _experienceService.ExperienceChanged -= OnExperienceChanged;
            _experienceService.LevelUp -= OnLevelUp;
            _experienceService.ExperienceGained -= OnExperienceGained;
        }
        
        _barTween?.Kill();
    }
    
    private void UpdateUI()
    {
        if (_experienceService == null) return;
        
        if (levelText != null)
            levelText.text = $"Уровень {_experienceService.CurrentLevel}";
            
        if (experienceText != null)
        {
            if (_experienceService.IsMaxLevel)
            {
                experienceText.text = "МАКС";
            }
            else
            {
                experienceText.text = $"{_experienceService.ExperienceToNextLevel} до след. уровня";
            }
        }
        
        if (experienceBar != null)
        {
            experienceBar.fillAmount = _experienceService.ExperienceProgress;
        }
    }
    
    private void OnLevelChanged(int newLevel)
    {
        UpdateUI();
    }
    
    private void OnExperienceChanged(int currentExp, int requiredExp)
    {
        UpdateExperienceBar();
        UpdateExperienceText();
    }
    
    private void OnLevelUp(int newLevel)
    {
        ShowLevelUpNotification(newLevel);
        AnimateExperienceBar();
    }
    
    private void OnExperienceGained(int adjustedAmount, int originalAmount)
    {
        ShowExperienceGainNotification(adjustedAmount);
        AnimateExperienceBar();
    }
    
    private void UpdateExperienceBar()
    {
        if (experienceBar == null) return;
        
        _barTween?.Kill();
        _barTween = experienceBar.DOFillAmount(_experienceService.ExperienceProgress, barAnimationDuration)
            .SetEase(Ease.OutQuart)
            .SetUpdate(true);
    }
    
    private void UpdateExperienceText()
    {
        if (experienceText == null) return;
        
        if (_experienceService.IsMaxLevel)
        {
            experienceText.text = "МАКС";
        }
        else
        {
            experienceText.text = $"{_experienceService.ExperienceToNextLevel} до след. уровня";
        }
    }
    
    private void AnimateExperienceBar()
    {
        if (experienceBar == null) return;
        _punch?.Kill(true);
        _punch = experienceBar.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 5, 0.5f)
            .SetUpdate(true);
    }
    
    private void ShowLevelUpNotification(int newLevel)
    {
        if (levelUpNotification == null) return;
        
        if (levelUpText != null)
            levelUpText.text = $"УРОВЕНЬ {newLevel}!";
            
        levelUpNotification.SetActive(true);
        
        var canvasGroup = levelUpNotification.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = levelUpNotification.AddComponent<CanvasGroup>();
            
        canvasGroup.alpha = 0f;
        levelUpNotification.transform.localScale = Vector3.zero;
        
        var sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1f, 0.3f).SetUpdate(true));
        sequence.Join(levelUpNotification.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true));
        sequence.AppendInterval(notificationDuration - 0.6f);
        sequence.Append(canvasGroup.DOFade(0f, 0.3f).SetUpdate(true));
        sequence.SetUpdate(true);
        sequence.OnComplete(() => levelUpNotification.SetActive(false));
    }
    
    private void ShowExperienceGainNotification(int amount)
    {
        if (experienceGainNotification == null) return;
        
        if (experienceGainText != null)
            experienceGainText.text = $"+{amount} опыта";
            
        experienceGainNotification.SetActive(true);
        
        var canvasGroup = experienceGainNotification.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = experienceGainNotification.AddComponent<CanvasGroup>();
            
        canvasGroup.alpha = 0f;
        experienceGainNotification.transform.localScale = Vector3.one * 0.8f;
        
        var sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1f, 0.2f).SetUpdate(true));
        sequence.Join(experienceGainNotification.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetUpdate(true));
        sequence.AppendInterval(1f);
        sequence.Append(canvasGroup.DOFade(0f, 0.3f).SetUpdate(true));
        sequence.Join(experienceGainNotification.transform.DOScale(0.8f, 0.3f).SetUpdate(true));
        sequence.SetUpdate(true);
        sequence.OnComplete(() => experienceGainNotification.SetActive(false));
    }
} 