using UnityEngine;
using UnityEngine.UI;

namespace FaintFear
{
    public class FlashlightUI : MonoBehaviour
    {
        private Flashlight flashlight;

        public CanvasGroup flashlightUI;
        public GameObject isOn;
        public Image batteryGauge;
        public Image[] batteryIcons;

        private void Update()
        {
            // 🔒 Player 아직 없으면 대기
            if (PlayerStatus.Instance == null || flashlight == null)
                return;

            // 배터리 갯수
            BatteryCountUI();

            // 배터리 게이지
            batteryGauge.fillAmount =
                PlayerStatus.Instance.BatteryNormalized;

            // 손전등 on/off
            if (flashlight.IsOn)
            {
                isOn.SetActive(true);
                flashlightUI.alpha = 1f;
            }
            else
            {
                isOn.SetActive(false);
                flashlightUI.alpha = 0.3f;
            }
        }

        void BatteryCountUI()
        {
            int count = PlayerStatus.Instance.batteryCount;

            for (int i = 0; i < batteryIcons.Length; i++)
            {
                batteryIcons[i].gameObject.SetActive(i < count);
            }
        }

        // ⭐ GameManager에서 호출
        public void BindPlayer(GameObject player)
        {
            flashlight = player.GetComponentInChildren<Flashlight>();
        }
    }
}
