using FaintFear;
using TMPro;
using UnityEngine;

public class PushItemDetector : MonoBehaviour
{
    public Camera playerCamera;
    public TMP_Text crosshairText;
    public TMP_Text screenText;
    public float rayDistance = 5f;

    public PowerGaugePlayer gaugePlayer; //상태만 참조

    void Start()
    {
        HideTexts();
    }

    void Update()
    {
        //이미 밀기(게이지 충전) 중이면 텍스트 숨김
        if (gaugePlayer != null && gaugePlayer.IsCharging)
        {
            HideTexts();
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (!hit.collider.CompareTag("PushItem"))
            {
                HideTexts();
                return;
            }

            PushItem item = hit.collider.GetComponent<PushItem>();

            //이미 치운 오브젝트면 x
            if (item == null || item.isCleared)
            {
                HideTexts();
                return;
            }

            PushItemInfo info = hit.collider.GetComponent<PushItemInfo>();
            if (info != null)
            {
                ShowTexts(info.crosshairText, info.screenText);
                return;
            }
        }

        HideTexts();
    }

    void ShowTexts(string crosshair, string screen)
    {
        crosshairText.gameObject.SetActive(true);
        screenText.gameObject.SetActive(true);
        crosshairText.text = crosshair;
        screenText.text = screen;
    }

    void HideTexts()
    {
        crosshairText.gameObject.SetActive(false);
        screenText.gameObject.SetActive(false);
    }
}