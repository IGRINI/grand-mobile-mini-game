using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TestUpgradeUI : MonoBehaviour
{
    [SerializeField] private GameObject testPanel;
    [SerializeField] private TextMeshProUGUI testText;
    [SerializeField] private Button testButton;
    
    private void Start()
    {
        if (testButton != null)
            testButton.onClick.AddListener(TestShowPanel);
            
        if (testPanel != null)
            testPanel.SetActive(false);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestShowPanel();
        }
    }
    
    private void TestShowPanel()
    {
        Debug.Log("TestUpgradeUI: Показываем тестовую панель");
        
        if (testPanel != null)
        {
            testPanel.SetActive(true);
            
            var canvasGroup = testPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = testPanel.AddComponent<CanvasGroup>();
            
            Time.timeScale = 0f;
            Debug.Log($"Time.timeScale = {Time.timeScale}");
            
            StartCoroutine(AnimateAlpha(canvasGroup));
        }
    }
    
    private IEnumerator AnimateAlpha(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        float duration = 1f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            Debug.Log($"Alpha: {canvasGroup.alpha:F2}, UnscaledDeltaTime: {Time.unscaledDeltaTime:F4}");
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        Debug.Log("Анимация альфы завершена");
        
        yield return new WaitForSecondsRealtime(2f);
        
        Time.timeScale = 1f;
        testPanel.SetActive(false);
    }
} 