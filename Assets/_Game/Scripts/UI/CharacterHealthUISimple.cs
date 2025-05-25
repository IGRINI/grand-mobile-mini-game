using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CharacterHealthUISimple : MonoBehaviour, IHealthUI
{
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private AnimationCurve healthBarCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve textCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private float _currentDisplayedHealth;
    private Coroutine _healthBarCoroutine;
    private Coroutine _healthTextCoroutine;
    
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        float healthPercentage = currentHealth / maxHealth;
        
        if (_healthBarCoroutine != null)
        {
            StopCoroutine(_healthBarCoroutine);
        }
        
        if (_healthTextCoroutine != null)
        {
            StopCoroutine(_healthTextCoroutine);
        }
        
        _healthBarCoroutine = StartCoroutine(AnimateHealthBar(healthPercentage));
        _healthTextCoroutine = StartCoroutine(AnimateHealthText(currentHealth, maxHealth));
    }
    
    private IEnumerator AnimateHealthBar(float targetFillAmount)
    {
        float startFillAmount = healthBar.fillAmount;
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float curveValue = healthBarCurve.Evaluate(t);
            
            healthBar.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, curveValue);
            yield return null;
        }
        
        healthBar.fillAmount = targetFillAmount;
        _healthBarCoroutine = null;
    }
    
    private IEnumerator AnimateHealthText(float targetHealth, float maxHealth)
    {
        float startHealth = _currentDisplayedHealth;
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float curveValue = textCurve.Evaluate(t);
            
            _currentDisplayedHealth = Mathf.Lerp(startHealth, targetHealth, curveValue);
            healthText.text = $"{Mathf.RoundToInt(_currentDisplayedHealth)}/{Mathf.RoundToInt(maxHealth)}";
            yield return null;
        }
        
        _currentDisplayedHealth = targetHealth;
        healthText.text = $"{Mathf.RoundToInt(targetHealth)}/{Mathf.RoundToInt(maxHealth)}";
        _healthTextCoroutine = null;
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
        if (_healthBarCoroutine != null)
        {
            StopCoroutine(_healthBarCoroutine);
        }
        
        if (_healthTextCoroutine != null)
        {
            StopCoroutine(_healthTextCoroutine);
        }
    }
} 