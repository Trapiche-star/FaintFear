using UnityEngine;
using UnityEngine.UI;

public class PushGaugeUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void SetGauge(float value)
    {
        fillImage.fillAmount = (value);
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    public void ResetGauge()
    {
        fillImage.fillAmount = 0f;
    }
}
