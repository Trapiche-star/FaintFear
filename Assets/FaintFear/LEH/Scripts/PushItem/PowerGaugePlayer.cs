using UnityEngine;
using UnityEngine.UI;
using System;

namespace FaintFear
{
    public class PowerGaugePlayer : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject gaugePanel;
        [SerializeField] private Image fillImage;

        [Header("Settings")]
        [SerializeField] private float chargeTime = 2f;

        private float currentCharge = 0f;
        private bool isCharging = false;
        private Action onCompleteCallback;

        public bool IsCharging => isCharging;
        public bool HasRemainingCharge => currentCharge > 0f;

        void Awake()
        {
            HideGauge();
        }

        void Update()
        {
            if (isCharging)
            {
                // 게이지 충전
                currentCharge += Time.deltaTime / chargeTime;
                currentCharge = Mathf.Clamp01(currentCharge);

                if (fillImage != null)
                {
                    fillImage.fillAmount = currentCharge;
                }

                // 게이지 완료
                if (currentCharge >= 1f)
                {
                    CompleteCharging();
                }
            }
        }

        public void StartCharging(Action onComplete)
        {
            isCharging = true;
            currentCharge = 0f;
            onCompleteCallback = onComplete;

            ShowGauge();
            Debug.Log("[PowerGaugePlayer] Started charging");
        }

        public void StopCharging()
        {
            if (!isCharging) return;

            isCharging = false;
            currentCharge = 0f;

            HideGauge();
            Debug.Log("[PowerGaugePlayer] Stopped charging");
        }

        void CompleteCharging()
        {
            isCharging = false;
            Debug.Log("[PowerGaugePlayer] Charging complete!");

            // 콜백 실행
            onCompleteCallback?.Invoke();
            onCompleteCallback = null;

            // 잠시 후 UI 숨김
            Invoke(nameof(HideGauge), 0.3f);
        }

        void ShowGauge()
        {
            if (gaugePanel != null)
                gaugePanel.SetActive(true);

            if (fillImage != null)
                fillImage.fillAmount = 0f;
        }

        void HideGauge()
        {
            currentCharge = 0f;

            if (fillImage != null)
                fillImage.fillAmount = 0f;

            if (gaugePanel != null)
                gaugePanel.SetActive(false);
        }
    }
}