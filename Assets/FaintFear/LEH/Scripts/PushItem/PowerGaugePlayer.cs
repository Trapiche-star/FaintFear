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

        float currentCharge;

        bool isPushHeld;
        bool isTouchingPushItem;

        [SerializeField] private GameObject flashlight;

        public bool IsCharging { get; private set; }
        public bool HasRemainingCharge => currentCharge > 0f;

        //UI / 외부 제어용 (중요)
        public bool CanZoom =>
            isPushHeld && isTouchingPushItem && currentItem != null;

        PushItem currentItem;
        PlayerMove playerMove;

        void Awake()
        {
            playerMove = GetComponent<PlayerMove>();

            currentCharge = 0f;
            IsCharging = false;

            ShowGauge(false);
            SetGauge(0f);
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
        }

        // =========================
        // 입력 상태
        // =========================
        void OnPushStateChanged(bool isHeld)
        {
            isPushHeld = isHeld;

            if (flashlight == null) return;

            //상호작용 중 일때 배터리 소모 끄기, 오브젝트 비활성화
            if (isHeld)
            {
                PlayerStatus.Instance.isBatteryActive = false;
                flashlight.SetActive(false);
            }
            else
            {
                PlayerStatus.Instance.isBatteryActive = true;
                flashlight.SetActive(true);
            }
        }

        // =========================
        // 밀 수 있는 물체 감지
        // =========================
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

        // =========================
        // 게이지 처리
        // =========================
        void HandleCharge()
        {
            // 닿아있지 않으면 감소만
            if (!isTouchingPushItem || currentItem == null)
            {
                IsCharging = false;
                DrainCharge(false);
                return;
            }

            // V키 누르고 있을 때만 충전
            if (isPushHeld)
            {
                IsCharging = true;
                ShowGauge(true);
                currentCharge += Time.deltaTime / maxChargeTime;
            }
            else
            {
                IsCharging = false;
                DrainCharge(false);
            }

            currentCharge = Mathf.Clamp01(currentCharge);
            SetGauge(currentCharge);

            // 가득 차면 밀기
            if (currentCharge >= 1f)
            {
                currentItem.MoveToTarget();
                ResetGauge();
            }
        }

        // =========================
        // 게이지 감소
        // =========================
        void DrainCharge(bool showUI)
        {
            if (currentCharge <= 0f)
            {
                ResetGauge();
                return;
            }

            currentCharge -= Time.deltaTime * drainSpeed;
            currentCharge = Mathf.Clamp01(currentCharge);

            SetGauge(currentCharge);
            ShowGauge(showUI);
        }

        // =========================
        // 초기화
        // =========================
        void ResetGauge()
        {
            currentCharge = 0f;
            IsCharging = false;

            SetGauge(0f);
            ShowGauge(false);
        }
        void ShowGauge(bool show)
        {
            if (gaugeUI == null) return;
            gaugeUI.Show(show);
        }

        void SetGauge(float value)
        {
            if (gaugeUI == null) return;
            gaugeUI.SetGauge(value);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }
#endif
    }
}