using UnityEngine;
using UnityEngine.UI;

public class PushGaugeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gaugePanel;
    [SerializeField] private Image fillImage;

    [Header("Colors")]
    [SerializeField] private Color chargingColor = Color.yellow;
    [SerializeField] private Color completeColor = Color.green;

    void Awake()
    {
        // ⭐ fillAmount 초기화
        if (fillImage != null)
        {
            fillImage.fillAmount = 0f;
        }

        HideGauge();
    }

    public void ShowGauge()
    {
        if (gaugePanel != null)
        {
            gaugePanel.SetActive(true);
            Debug.Log("[PushGaugeUI] Gauge shown");
        }

        if (fillImage != null)
        {
            fillImage.fillAmount = 0f;
            fillImage.color = chargingColor;
        }
    }

    public void UpdateGauge(float progress)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = progress;

            Debug.Log($"[PushGaugeUI] Gauge updated: {progress * 100:F1}%");

            // 게이지가 거의 다 차면 색상 변경
            if (progress >= 0.95f)
            {
                fillImage.color = completeColor;
            }
            else
            {
                fillImage.color = chargingColor;
            }
        }
    }

    public void HideGauge()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = 0f;
        }

        if (gaugePanel != null)
        {
            gaugePanel.SetActive(false);
            Debug.Log("[PushGaugeUI] Gauge hidden");
        }
    }
}