using FaintFear;
using TMPro;
using UnityEngine;

public class PushItemDetector : MonoBehaviour
{
    [Header("Detect")]
    public float detectDistance = 3f;
    public LayerMask pushItemLayer;

    [Header("UI")]
    public TMP_Text crosshairText;
    public TMP_Text screenText;

    [Header("Reference")]
    public PowerGaugePlayer powerGaugePlayer; // Inspector에서 연결

    void Awake()
    {
        HideText();
    }

    void Update()
    {
        Detect();
    }

    void Detect()
    {
        //게이지가 남아 있거나 충전 중이면 무조건 텍스트 숨김
        if (powerGaugePlayer != null &&
            (powerGaugePlayer.IsCharging || powerGaugePlayer.HasRemainingCharge))
        {
            HideText();
            return;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, detectDistance, pushItemLayer))
        {
            HideText();
            return;
        }

        if (!hit.collider.CompareTag("PushItem"))
        {
            HideText();
            return;
        }

        PushItemInfo info = hit.collider.GetComponent<PushItemInfo>();
        PushItem item = hit.collider.GetComponent<PushItem>();

        if (info == null || item == null || item.isCleared)
        {
            HideText();
            return;
        }

        //텍스트 표시
        crosshairText.gameObject.SetActive(true);
        screenText.gameObject.SetActive(true);

        crosshairText.text = info.crosshairText;
        screenText.text = info.screenText;
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
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * detectDistance);
    }
#endif
}