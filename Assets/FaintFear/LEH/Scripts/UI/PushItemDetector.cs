/*using FaintFear;
using TMPro;
using UnityEngine;

public class PushItemDetector : MonoBehaviour
{
    [Header("Detect")]
    public float detectDistance = 3f;
    public LayerMask pushItemLayer;

    [Header("UI")]
    public TextMeshProUGUI crosshairText;
    public TextMeshProUGUI screenText;

    [Header("Reference")]
    public PowerGaugePlayer powerGaugePlayer;

    private PushItem currentPushItem;
    private PlayerMove playerMove;
    private bool isPushing = false;

    void Awake()
    {
        HideText();
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMove = player.GetComponent<PlayerMove>();
        }

        if (playerMove == null)
        {
            Debug.LogError("[PushItemDetector] PlayerMove not found!");
        }
    }

    void OnEnable()
    {
        if (playerMove != null)
        {
            // ⭐ PlayerMove의 Push 이벤트 구독
            playerMove.OnPushEvent += OnPushInput;
        }
    }

    void OnDisable()
    {
        if (playerMove != null)
        {
            playerMove.OnPushEvent -= OnPushInput;
        }
    }

    void Update()
    {
        Detect();
    }

    void Detect()
    {
        // 게이지가 충전 중이면 감지 중단
        if (powerGaugePlayer != null && powerGaugePlayer.IsCharging)
        {
            return;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, detectDistance, pushItemLayer))
        {
            currentPushItem = null;
            HideText();
            return;
        }

        if (!hit.collider.CompareTag("PushItem"))
        {
            currentPushItem = null;
            HideText();
            return;
        }

        PushItem item = hit.collider.GetComponent<PushItem>();
        PushItemInfo info = hit.collider.GetComponent<PushItemInfo>();

        if (item == null || info == null || item.isCleared)
        {
            currentPushItem = null;
            HideText();
            return;
        }

        // ⭐ 상호작용 가능한 아이템 발견
        currentPushItem = item;
        ShowText(info);
    }

    // ⭐ V키 입력 처리
    void OnPushInput(bool isPressing)
    {
        if (currentPushItem == null || currentPushItem.isCleared)
        {
            if (isPushing)
            {
                StopPushing();
            }
            return;
        }

        if (isPressing && !isPushing)
        {
            // V키 누르기 시작
            StartPushing();
        }
        else if (!isPressing && isPushing)
        {
            // V키 떼기
            StopPushing();
        }
    }

    void StartPushing()
    {
        isPushing = true;
        Debug.Log("[PushItemDetector] Started pushing");

        if (powerGaugePlayer != null)
        {
            // ⭐ 게이지 충전 시작
            powerGaugePlayer.StartCharging(OnPushComplete);
        }
    }

    void StopPushing()
    {
        if (!isPushing) return;

        isPushing = false;
        Debug.Log("[PushItemDetector] Stopped pushing");

        if (powerGaugePlayer != null)
        {
            // ⭐ 게이지 충전 중단
            powerGaugePlayer.StopCharging();
        }
    }

    // ⭐ 게이지가 다 찼을 때 호출되는 콜백
    void OnPushComplete()
    {
        Debug.Log("[PushItemDetector] Push complete!");

        if (currentPushItem != null && !currentPushItem.isCleared)
        {
            // ⭐ 오브젝트 이동 실행
            currentPushItem.MoveToTarget();
            currentPushItem = null;
            HideText();
        }

        isPushing = false;
    }

    void ShowText(PushItemInfo info)
    {
        if (crosshairText != null)
        {
            crosshairText.gameObject.SetActive(true);
            crosshairText.text = info.crosshairText;
        }

        if (screenText != null)
        {
            screenText.gameObject.SetActive(true);
            screenText.text = info.screenText;
        }
    }

    void HideText()
    {
        if (crosshairText != null)
        {
            crosshairText.text = "";
            crosshairText.gameObject.SetActive(false);
        }

        if (screenText != null)
        {
            screenText.text = "";
            screenText.gameObject.SetActive(false);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * detectDistance);
    }
#endif
}*/