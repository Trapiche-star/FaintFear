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

        //외부 공개 상태
        public bool IsCharging { get; private set; }
        public bool HasRemainingCharge => currentCharge > 0f;

        PushItem currentItem;
        PlayerMove playerMove;

        void Awake()
        {
            playerMove = GetComponent<PlayerMove>();

            if (playerCamera == null)
                playerCamera = Camera.main;

            defaultFOV = playerCamera.fieldOfView;

            IsCharging = false;
            currentCharge = 0f;

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
                if (!hit.CompareTag("PushItem"))
                    continue;

                PushItem item = hit.GetComponent<PushItem>();
                if (item == null || item.isCleared)
                    continue;

                currentItem = item;
                isTouchingPushItem = true;
                return;
            }
        }
        #endregion

        #region Charge
        void HandleCharge()
        {
            // PushItem과 접촉 중이 아니면 충전 불가
            if (!isTouchingPushItem || currentItem == null)
            {
                IsCharging = false;
                DrainCharge();
                return;
            }

            if (isPushHeld)
            {
                IsCharging = true;
                gaugeUI.Show(true);
                currentCharge += Time.deltaTime / maxChargeTime;
            }
            else
            {
                IsCharging = false;
                DrainCharge();
            }

            currentCharge = Mathf.Clamp01(currentCharge);
            gaugeUI.SetGauge(currentCharge);

            // 충전 완료
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

            if (currentCharge <= 0f)
            {
                ResetGauge();
                return;
            }

            currentCharge = Mathf.Clamp01(currentCharge);
            gaugeUI.SetGauge(currentCharge);
            gaugeUI.Show(true);
        }

        void ResetGauge()
        {
            currentCharge = 0f;
            IsCharging = false;
            gaugeUI.SetGauge(0f);
            gaugeUI.Show(false);
        }
        #endregion

        #region Camera
        void HandleZoom()
        {
            float targetFOV =
                (isPushHeld && isTouchingPushItem)
                ? zoomFOV
                : defaultFOV;

            playerCamera.fieldOfView =
                Mathf.Lerp(
                    playerCamera.fieldOfView,
                    targetFOV,
                    Time.deltaTime * zoomSpeed
                );
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