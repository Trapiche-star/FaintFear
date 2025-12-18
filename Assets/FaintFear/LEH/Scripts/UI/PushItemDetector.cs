using UnityEngine;
using TMPro;

public class PushItemDetector : MonoBehaviour
{
    public Camera playerCamera;
    public TMP_Text crosshairText;
    public TMP_Text screenText;
    public float rayDistance = 5f;

    private PushItemInfo currentItem;

    void Start()
    {
        crosshairText.gameObject.SetActive(false);
        screenText.gameObject.SetActive(false);
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("PushItem"))
            {
                currentItem = hit.collider.GetComponent<PushItemInfo>();

                if (currentItem != null)
                {
                    //여기서만 켠다
                    crosshairText.gameObject.SetActive(true);
                    screenText.gameObject.SetActive(true);

                    crosshairText.text = currentItem.crosshairText;
                    screenText.text = currentItem.screenText;
                    return;
                }
            }
        }

        //PushItem을 안 보고 있을 때
        currentItem = null;
        crosshairText.gameObject.SetActive(false);
        screenText.gameObject.SetActive(false);
    }
}