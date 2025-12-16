using UnityEngine;
using UnityEngine.UI;

namespace FaintFear
{
    /// <summary>
    /// 손전등UI를 관리하는 클래스
    /// </summary>
    public class FlashlightUI : MonoBehaviour
    {
        #region Variables
        private Flashlight flashlight;
        public CanvasGroup flashlightUI;
        public GameObject isOn;     //손전등 on/off
        public Image batteryGauge;  //배터리 게이지
        public Image[] batteryIcons;    //소지 중인 배터리 갯수 
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            flashlight = FindFirstObjectByType<Flashlight>();
        }
        private void Update()
        {
            //배터리 갯수
            BatteryCountUI();

            //배터리 게이지
            batteryGauge.fillAmount =
            PlayerStatus.Instance.BatteryNormalized;

            //손전등 on/off
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
        #endregion

        #region Custom Method
        void BatteryCountUI()
        {
            int count = PlayerStatus.Instance.batteryCount;

            for (int i = 0; i < batteryIcons.Length; i++)
            {
                batteryIcons[i].gameObject.SetActive(i < count);
            }
        }
        #endregion
    }
}