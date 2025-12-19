using UnityEngine;
using UnityEngine.UI;

public class PushGaugeUI : MonoBehaviour
{
    public Image thickGauge; // 굵은 원 (Filled Image)

    public void SetGauge(float value)
    {
        thickGauge.fillAmount = value;
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }
}