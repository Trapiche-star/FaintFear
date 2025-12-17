using UnityEngine;
using UnityEngine.UI;
public class GaugeUI : MonoBehaviour
{
    public Image thinCircle;
    public Image thickCircle;

    public void Show(bool show)
    {
        thinCircle.gameObject.SetActive(show);
        thickCircle.gameObject.SetActive(show);
    }

    public void SetGauge(float value)
    {
        thickCircle.fillAmount = value;
    }

}
