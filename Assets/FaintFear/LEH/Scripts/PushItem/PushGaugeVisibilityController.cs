using UnityEngine;

namespace FaintFear
{
    public class PushGaugeVisibilityController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PowerGaugePlayer powerGaugePlayer;
        [SerializeField] private PushGaugeUI gaugeUI;

        void Awake()
        {
            if (gaugeUI != null)
                gaugeUI.Show(false);
        }

        void Update()
        {
            if (powerGaugePlayer == null || gaugeUI == null)
                return;

            // UI 표시 조건:
            // 1. V키 누르고 있음
            // 2. 밀 수 있는 대상에 닿아 있음
            bool shouldShow =
                powerGaugePlayer.IsCharging;

            gaugeUI.Show(shouldShow);
        }
    }
}
