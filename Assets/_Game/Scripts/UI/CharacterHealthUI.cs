using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CharacterHealthUI : MonoBehaviour, IHealthUI
{
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease healthBarEase = Ease.OutQuart;
    [SerializeField] private Ease textEase = Ease.OutBack;
    
    private float _currentDisplayedHealth;
    private Tween _healthBarTween;
    private Tween _healthTextTween;
    
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        float healthPercentage = currentHealth / maxHealth;
        
        _healthBarTween?.Kill();
        _healthTextTween?.Kill();
        
        _healthBarTween = healthBar.DOFillAmount(healthPercentage, animationDuration)
            .SetEase(healthBarEase);
        
        _healthTextTween = DOTween.To(
            () => _currentDisplayedHealth,
            x => {
                _currentDisplayedHealth = x;
                healthText.text = $"{Mathf.RoundToInt(x)}/{Mathf.RoundToInt(maxHealth)}";
            },
            currentHealth,
            animationDuration
        ).SetEase(textEase);
    }
    
    public void SetCharacterName(string name)
    {
        if (characterNameText != null)
        {
            characterNameText.text = name;
        }
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        _healthBarTween?.Kill();
        _healthTextTween?.Kill();
    }
} 