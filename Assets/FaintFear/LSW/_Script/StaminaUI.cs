using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    private Slider staminaSlider;

    private void Awake()
    {
        staminaSlider = GetComponentInChildren<Slider>();
    }
}
