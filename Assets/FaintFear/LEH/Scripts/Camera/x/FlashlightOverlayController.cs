using UnityEngine;

namespace FaintFear
{
    public class FlashlightOverlayController : MonoBehaviour
    {
        public PowerGaugePlayer powerGauge;

        Camera cam;

        void Awake()
        {
            cam = GetComponent<Camera>();
        }

        void Update()
        {
            if (powerGauge == null) return;

            // 줌 중에는 손전등 안 보이게
            cam.enabled = !powerGauge.CanZoom;
        }
    }
}
