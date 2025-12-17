using UnityEngine;
using UnityEngine.UI;

public class PushGaugeUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void SetGauge(float value)
    {
        fillImage.fillAmount = Mathf.Clamp01(value);
    }

    public void ResetGauge()
    {
        fillImage.fillAmount = 0f;
    }
}
