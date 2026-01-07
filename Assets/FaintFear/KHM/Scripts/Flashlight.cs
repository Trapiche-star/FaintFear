using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 손전등 기능을 담당하는 스크립트
    /// </summary>
    public class Flashlight : MonoBehaviour
    {
        [Header("Light")]
        public Light spotLight;
        [SerializeField] private GameObject flashlightModel;

        [Header("Battery")]
        [SerializeField] private float batteryDrainRate = 10f; // 1초당 배터리 소모량

        [Header("Sound IDs (SoundManager 기준)")]

        //+
        [Header("Optional: Toggle Sound")]
        [SerializeField] private string toggleSFX = "SFX_Flashlight"; // 켜기/끄기 공용
        //
        [SerializeField] private string turnOnSFX = "SFX_Flashlight_On";
        [SerializeField] private string turnOffSFX = "SFX_Flashlight_Off";
        [SerializeField] private string batteryEmptySFX = "SFX_Flashlight_Empty";

        private PlayerMove playerMove;

        private bool wasOnBeforePush = false; // ⭐ 밀기 전 상태 저장

        private bool isOn = false;

        public bool IsOn => isOn;

        private void Start()
        {
            isOn = false;
            spotLight.enabled = false;

            PlayerStatus.Instance.isBatteryActive = true;
        }

        private void Update()
        {
            if (isOn)
            {
                DrainBattery();
            }
        }
        private void OnEnable()
        {
            playerMove = GetComponentInParent<PlayerMove>();
            if (playerMove != null)
                playerMove.OnPushEvent += OnPushStateChanged;
        }

        private void OnDisable()
        {
            if (playerMove != null)
                playerMove.OnPushEvent -= OnPushStateChanged;
        }
        #region Public Method
        public void ToggleLight()
        {
            // 배터리도 없고, 충전된 것도 없으면 사용 불가
            if (PlayerStatus.Instance.batteryCount <= 0 &&
                PlayerStatus.Instance.currentBattery <= 0f)
            {
                TurnOff(true);
                return;
            }

            if (isOn)
            {
                TurnOff(false);
            }
            else
            {
                TurnOn();
            }

            //+ 켜기/끄기 시 동일 사운드 재생
            if (!string.IsNullOrEmpty(toggleSFX))
                SoundManager.Instance.PlaySFX(toggleSFX);
            //
        }
        #endregion

        #region Private Method
        void TurnOn()
        {
            isOn = true;
            spotLight.enabled = true;

            if (!string.IsNullOrEmpty(turnOnSFX))
                SoundManager.Instance.PlaySFX(turnOnSFX);
        }

        void TurnOff(bool isBatteryEmpty)
        {
            isOn = false;
            spotLight.enabled = false;

            if (isBatteryEmpty)
            {
                if (!string.IsNullOrEmpty(batteryEmptySFX))
                    SoundManager.Instance.PlaySFX(batteryEmptySFX);
            }
            else
            {
                if (!string.IsNullOrEmpty(turnOffSFX))
                    SoundManager.Instance.PlaySFX(turnOffSFX);
            }
        }

        // 손전등 배터리 소모
        void DrainBattery()
        {
            if (!PlayerStatus.Instance.isBatteryActive)
                return;

            PlayerStatus.Instance.currentBattery -= batteryDrainRate * Time.deltaTime;

            if (PlayerStatus.Instance.currentBattery <= 0f)
            {
                PlayerStatus.Instance.currentBattery = 0f;

                if (PlayerStatus.Instance.UseBattery())
                {
                    // 배터리 교체 성공 → 그대로 유지
                }
                else
                {
                    TurnOff(true);
                }
            }
        }

        private void OnPushStateChanged(bool isPushing)
        {
            if (isPushing)
            {
                wasOnBeforePush = isOn;

                TurnOff(false);

                if (flashlightModel != null)
                    flashlightModel.SetActive(false);
            }
            else
            {
                if (flashlightModel != null)
                    flashlightModel.SetActive(true);

                if (wasOnBeforePush)
                    TurnOn();
            }
        }
        #endregion
    }
}
