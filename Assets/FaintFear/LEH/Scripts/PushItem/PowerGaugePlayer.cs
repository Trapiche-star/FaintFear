using UnityEngine;

namespace FaintFear
{
    public class PowerGaugePlayer : MonoBehaviour
    {
        [Header("UI")]
        public PushGaugeUI gaugeUI;

        [Header("Charge Settings")]
        public float maxChargeTime = 2f;
        public float drainSpeed = 0.4f;

        [Header("Detect Settings")]
        public float detectRadius = 0.8f;
        public LayerMask pushItemLayer;

        [Header("Camera Zoom")]
        public Camera playerCamera;
        public float zoomFOV = 40f;
        public float zoomSpeed = 10f;

        float currentCharge;
        float defaultFOV;

        bool isPushHeld;
        bool isTouchingPushItem;

        public bool IsCharging { get; private set; } // 🔥 추가된 핵심

        PushItem currentItem;
        PlayerMove playerMove;

        void Awake()
        {
            playerMove = GetComponent<PlayerMove>();

            if (playerCamera == null)
                playerCamera = Camera.main;

            defaultFOV = playerCamera.fieldOfView;

            gaugeUI.Show(false);
            gaugeUI.SetGauge(0f);
        }

        void OnEnable()
        {
            playerMove.OnPushEvent += OnPushStateChanged;
        }

        void OnDisable()
        {
            playerMove.OnPushEvent -= OnPushStateChanged;
        }

        void Update()
        {
            DetectPushItem();
            HandleCharge();
            HandleZoom();
        }

        #region Input
        void OnPushStateChanged(bool isHeld)
        {
            isPushHeld = isHeld;
        }
        #endregion

        #region Detect
        void DetectPushItem()
        {
            isTouchingPushItem = false;
            currentItem = null;

            Collider[] hits = Physics.OverlapSphere(
                transform.position,
                detectRadius,
                pushItemLayer
            );

            foreach (var hit in hits)
            {
                if (hit.CompareTag("PushItem"))
                {
                    currentItem = hit.GetComponent<PushItem>();
                    if (currentItem != null && !currentItem.isCleared)
                    {
                        isTouchingPushItem = true;
                        return;
                    }
                }
            }
        }
        #endregion

        #region Charge
        void HandleCharge()
        {
            // 접촉 안 하면 충전 불가
            if (!isTouchingPushItem || currentItem == null)
            {
                IsCharging = false; // 🔥
                DrainCharge();
                return;
            }

            if (isPushHeld)
            {
                IsCharging = true;  // 🔥
                gaugeUI.Show(true);
                currentCharge += Time.deltaTime / maxChargeTime;
            }
            else
            {
                IsCharging = false; // 🔥
                DrainCharge();
            }

            currentCharge = Mathf.Clamp01(currentCharge);
            gaugeUI.SetGauge(currentCharge);

            if (currentCharge >= 1f)
            {
                currentItem.MoveToTarget();
                ResetGauge();
            }
        }

        void DrainCharge()
        {
            if (currentCharge <= 0f)
            {
                ResetGauge();
                return;
            }

            currentCharge -= Time.deltaTime * drainSpeed;
            currentCharge = Mathf.Clamp01(currentCharge);
            gaugeUI.SetGauge(currentCharge);

            if (currentCharge > 0f)
                gaugeUI.Show(true);
        }

        void ResetGauge()
        {
            currentCharge = 0f;
            IsCharging = false; // 🔥
            gaugeUI.SetGauge(0f);
            gaugeUI.Show(false);
        }
        #endregion

        #region Camera
        void HandleZoom()
        {
            float targetFOV = (isPushHeld && isTouchingPushItem)
                ? zoomFOV
                : defaultFOV;

            playerCamera.fieldOfView =
                Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        }
        #endregion

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }
#endif
    }
}